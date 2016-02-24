using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using GetCompliance.Domain.Infrastructure;

namespace GetCompliance.Domain
{
    public class UnparsedEmail : EntityBase
    {
        public UnparsedEmail() { }

        public UnparsedEmail(byte[] bytes)
        {
            var filenameSize = GetFilenameSize(bytes.First());
            File = GetFile(bytes, filenameSize);
            Filename = GetFilename(bytes, filenameSize);
        }

        private static int GetFilenameSize(byte b)
        {
            return Convert.ToInt32(b);
        }

        private static string GetFilename(IEnumerable<byte> byteArray, int filenameSize)
        {
            return Encoding.UTF8.GetString(byteArray.Skip(1).Take(filenameSize).ToArray());
        }

        private static Stream GetFile(IEnumerable<byte> byteArray, int filenameSize)
        {
            return new MemoryStream(byteArray.Skip(1 + filenameSize).ToArray());
        }

        public string Filename { get; set; }
        public Stream File { get; set; }

        public byte[] SerializeAsBytes()
        {
            var messageBytes = new List<byte>
            {
                Convert.ToByte(Filename.Length)
            };
            messageBytes.AddRange(Encoding.UTF8.GetBytes(Filename));
            messageBytes.AddRange(File.ReadAsBytes());

            return messageBytes.ToArray();
        }

        protected override void Validate()
        {
            if (Filename.Length > 255)
            {
                AddBrokenRule(UnparsedEmailRules.FilenameMaxLength);
            }
        }
    }
}