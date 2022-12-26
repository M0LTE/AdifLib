using System;
using System.Collections.Generic;
using System.Text;

namespace M0LTE.AdifLib
{
    public class AdifFile
    {
        public AdifHeaderRecord Header { get; set; }
        public IList<AdifContactRecord> Records { get; set; } = new List<AdifContactRecord>();

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

                var data = parts[1];

                int cursor = 0, dataCur = 0;
                var state = State.LookingForStartOfField;
                var fieldNameBuilder = new StringBuilder();
                var fieldLengthBuilder = new StringBuilder();
                var dataBuilder = new StringBuilder();
                int dataLength = 0;
                var records = new Dictionary<string, string>();
                while (data.Length > cursor)
                {
                    var c = data[cursor];

                    if (state == State.LookingForStartOfField)
                    {
                        if (c == '<')
                        {
                            state = State.ReadingFieldName;
                            fieldNameBuilder.Clear();
                        }
                        cursor++;
                        continue;
                    }
                    else if (state == State.ReadingFieldName)
                    {
                        if (c == ':')
                        {
                            state = State.ReadingDataLength;
                            fieldLengthBuilder.Clear();
                        }
                        else if (c == '>' && fieldNameBuilder.ToString().Equals("eor", StringComparison.OrdinalIgnoreCase))
                        {
                            var contact = new AdifContactRecord();
                            foreach (var record in records)
                            {
                                contact.Fields.Add(record.Key, record.Value);
                            }
                            adifFile.Records.Add(contact);
                            records.Clear();

                            state = State.LookingForStartOfField;
                        }
                        else
                        {
                            fieldNameBuilder.Append(c);
                        }
                        cursor++;
                        continue;
                    }
                    else if (state == State.ReadingDataLength)
                    {
                        if (c == '>')
                        {
                            state = State.ReadingData;
                            dataLength = int.Parse(fieldLengthBuilder.ToString());
                            dataCur = 0;
                            dataBuilder.Clear();
                        }
                        else
                        {
                            fieldLengthBuilder.Append(c);
                        }
                        cursor++;
                        continue;
                    }
                    else if (state == State.ReadingData)
                    {
                        if (dataCur == dataLength)
                        {
                            records.Add(fieldNameBuilder.ToString(), dataBuilder.ToString());
                            fieldLengthBuilder.Clear();
                            state = State.LookingForStartOfField;
                        }
                        else
                        {
                            dataBuilder.Append(c);
                            dataCur++;
                        }
                        cursor++;
                        continue;
                    }
                }

                return true;
            }

            adifFile = null;
            return false;
        }

        
        enum State
        {
            LookingForStartOfField,
            ReadingFieldName,
            ReadingDataLength,
            ReadingData
        }


    }
}