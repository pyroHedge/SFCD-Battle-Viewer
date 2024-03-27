using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace PyroPatchViewer
{
    class BinaryFile
    {
        public string FilePath { get; set; }
        public string Name { get { return Path.GetFileName(FilePath); } }
        public DateTime LastRead { get; private set; }
        public List<byte> Data { get; set; } = new List<byte>();
        public int Length { get { return Data.Count(); } }
        public bool Dirty { get; set; }

        public BinaryFile() { }

        public BinaryFile(string filePath)
        {
            Read(filePath);
        }


        //--------------------------------------------------------------
        // Read and Write the Data
        //--------------------------------------------------------------

        /// <summary>
        /// Read the file and load into memory. Updates the FilePath to the open file.
        /// </summary>
        /// <param name="filePath">File path to read.</param>
        /// <returns>TRUE if the file successfully loaded.</returns>
        public bool Read(string filePath)
        {
            bool error = false;
            try
            {
                FileInfo fi = new FileInfo(filePath);
                if (fi.Length > Math.Pow(2, 24))
                    throw new InvalidOperationException("Rom can only be 16MB or less in size.");
                Data = File.ReadAllBytes(filePath).ToList();
                FilePath = filePath;
                LastRead = DateTime.Now;
                Dirty = false;
            }
            catch (Exception e)
            {
                error = true;
                MessageBox.Show("Unable to read file " + filePath + Environment.NewLine + Environment.NewLine + e.Message, e.Source, MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            return !error;
        }

        /// <summary>
        /// Reload rom data directly from the filepath without saving. This effectively overwrites all changes.
        /// </summary>
        /// <returns>TRUE if the reload was successful. FALSE otherwise.</returns>
        public bool Reload()
        {
            return Read(FilePath);
        }

        /// <summary>
        /// overwrite the current file.
        /// </summary>
        /// <returns>TRUE if the file successfully loaded.</returns>
        public bool Save()
        {
            return Write(FilePath);
        }

        /// <summary>
        /// Write the file to the selected location. Updates the FilePath to the save location.
        /// </summary>
        /// <param name="filePath">File path to which to write or overwrite.</param>
        /// <returns>TRUE if the file successfully wrote to the filepath.</returns>
        public bool Write(string filePath)
        {
            bool error = false;
            try
            {
                File.WriteAllBytes(filePath, Data.ToArray());
                FilePath = filePath;
                Dirty = false;
            }
            catch (Exception e)
            {
                error = true;
                MessageBox.Show("Unable to write to file " + filePath + Environment.NewLine + Environment.NewLine + e.Message, e.Source, MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            return !error;
        }



        //--------------------------------------------------------------
        // Data Acquisition
        //--------------------------------------------------------------

        /// <summary>
        /// Return the specified bytes.
        /// </summary>
        /// <param name="location">Starting index for the location of the bytes to return.</param>
        /// <param name="length">The number of bytes to return.</param>
        /// <returns>Returns a single byte of 0 if there is an error. Otherwise, returns the specified bytes.</returns>
        public List<byte> GetData(int location, int length)
        {
            //if (location + length > Data.Length)
            //    return new byte[] { 0 };
            //else
            //    return Data.Skip(location).Take(length).ToArray();
            return Data.Take(new Range(location, location + length - 1)).ToList();
        }

        /// <summary>
        /// Return the specified bytes.
        /// </summary>
        /// <param name="location">Starting index for the location of the bytes to return.</param>
        /// <param name="length">The number of bytes to return.</param>
        /// <returns>Returns a single byte of 0 if there is an error. Otherwise, returns the specified bytes.</returns>
        public ByteChunk GetByteChunk(int location, int length)
        {
            return new ByteChunk(Data, location, length);
        }

        /// <summary>
        /// Find the location matching the provided searchBytes, where a NULL entry is a wildcard and can be matched on any value.
        /// </summary>
        /// <param name="searchBytes">Bytes to find in the rom. A NULL byte is a wildcard and can be matched on any value.</param>
        /// <param name="startLocation">Start searching at this byte location within the data.</param>
        /// <returns></returns>
        public int Find(byte?[] searchBytes, int startLocation = 0)
        {
            //Create BoyerMoore table
            BoyerMoore boyerMoore = new BoyerMoore(searchBytes);
            int len = searchBytes.Length - 1;
            int limit = Data.Count - searchBytes.Length + 1;
            for (int i = startLocation; i < limit;)
            {
                //if (i >= 0xB2440 && i < 0xB2460)
                //    Debugger.Break();
                int n = len;
                for (; n >= 0; n--)
                {
                    if (searchBytes[n] != null && searchBytes[n] != Data[i + n])
                    {
                        i += boyerMoore.Table[n, Data[i + n]];
                        break;
                    }
                }
                if (n == -1)
                {
                    return i;
                }
            }
            return -1;
        }



        //--------------------------------------------------------------
        // Data Edits
        //--------------------------------------------------------------

        /// <summary>
        /// Set the bytes at the specified location equal to the new value.
        /// </summary>
        /// <param name="location">Starting index for the location of the bytes to edit.</param>
        /// <param name="newValue">The new bytes.</param>
        /// <returns>TRUE if the edit was successful. FALSE otherwise.</returns>
        public bool Overwrite(int location, IEnumerable<byte> newValues)
        {
            //int n = location;
            //foreach (byte value in newValues)
            //{
            //    Data[location + n] = value;
            //    n++;
            //}
            Data.EnsureCapacity(location + newValues.Count());
            foreach (byte value in newValues)
            {
                if (Data.Count > location)
                {
                    Data[location] = value;
                }
                else
                {
                    Data.Add(value);
                }
                location++;
            }
            Dirty = true;
            return true;
        }

        /// <summary>
        /// Insert new bytes at the specified location.
        /// </summary>
        /// <param name="location">Location of where to insert the new bytes.</param>
        /// <param name="newValues">The new bytes to insert.</param>
        /// <returns>TRUE if the edit was successful. FALSE otherwise.</returns>
        public bool Insert(int location, IEnumerable<byte> newValues)
        {
            Data.InsertRange(location, newValues);
            Dirty = true;
            return true;
        }

        /// <summary>
        /// Add the specified bytes to the end of the file.
        /// </summary>
        /// <param name="newValues">The new bytes to insert.</param>
        /// <returns>TRUE if the edit was successful. FALSE otherwise.</returns>
        public bool Append(IEnumerable<byte> newValues)
        {
            Data.AddRange(newValues);
            Dirty = true;
            return true;
        }

        /// <summary>
        /// Delete bytes at the specified location.
        /// </summary>
        /// <param name="start">Location of where to delete.</param>
        /// <param name="count">The number of bytes to delete.</param>
        /// <returns>TRUE if the delete was successful. FALSE otherwise.</returns>
        public bool Delete(int location, int count)
        {
            Data.RemoveRange(location, count);
            Dirty = true;
            return true;
        }

        /// <summary>
        /// Overwrite bytes at location with the fillValue. Will append the rom to accomodate a fill.
        /// </summary>
        /// <param name="location">Location of where to overwrite.</param>
        /// <param name="count">The number of bytes to overwrite.</param>
        /// <param name="fillValue">The new byte which replaces the existing bytes.</param>
        /// <returns>TRUE if the fill was successful. FALSE otherwise.</returns>
        public bool Fill(int location, int count, byte fillValue)
        {
            Data.EnsureCapacity(location + count);
            int fillStop = Math.Min(Data.Count, location + count);
            for (int i = location; i < fillStop; i++)
            {
                Data[location + i] = fillValue;
            }
            if (Data.Count < location + count)
            {
                for (int i = fillStop; i < location + count; i++)
                {
                    Data.Add(fillValue);
                }
            }
            Dirty = true;
            return true;
        }

        /// <summary>
        /// Delete all bytes on or after location.
        /// </summary>
        /// <param name="location">All bytes on or after this are deleted.</param>
        /// <returns>TRUE if the bytes was successfully deleted.</returns>
        public bool Truncate(int location)
        {
            Data.RemoveRange(location, Data.Count - location);
            Dirty = true;
            return true;
        }

    }
}
