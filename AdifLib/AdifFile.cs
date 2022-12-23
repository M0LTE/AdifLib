using System;
using System.Collections.Generic;

namespace M0LTE.AdifLib
{
    public class AdifFile
    {
        public AdifHeaderRecord Header { get; set; }
        public IList<AdifContactRecord> Records { get; set; }

        public static bool TryParse(string adif, out AdifFile adifFile)
        {
            if (string.IsNullOrWhiteSpace(adif))
            {
                throw new ArgumentException("adif is null or empty");
            }

            var parts = adif.Split(new string[] { "<EOH>" }, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length == 2)
            {
                adifFile = new AdifFile();

                if (!AdifHeaderRecord.TryParse(parts[0], out var header, out _))
                {
                    adifFile = null;
                    return false;
                }

                adifFile.Header = header;

                foreach (var line in parts[1].Split('\r', '\n'))
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    if (AdifContactRecord.TryParse(line, out var record, out _))
                    {
                        if (adifFile.Records == null)
                        {
                            adifFile.Records = new List<AdifContactRecord>();
                        }

                        adifFile.Records.Add(record);
                    }
                }

                return true;
            }

            adifFile = null;
            return false;
        }
    }
}