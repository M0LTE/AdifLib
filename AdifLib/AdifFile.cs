using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace M0LTE.AdifLib
{
    public class AdifFile
    {
        public AdifHeaderRecord Header { get; set; }
        public IList<AdifContactRecord> Records { get; set; } = new List<AdifContactRecord>();

        public static bool TryParse(string adif, out AdifFile adifFile)
        {
            return TryParse(adif, out adifFile, out _);
        }

        public static bool TryParse(string adif, out AdifFile adifFile, out string reason)
        {
            if (string.IsNullOrWhiteSpace(adif))
            {
                reason = "No ADIF data";
                adifFile = null;
                return false;
            }

            var parts = Regex.Split(adif, "<eoh>", RegexOptions.IgnoreCase);

            if (parts.Length == 2)
            {
                adifFile = new AdifFile();

                if (!AdifHeaderRecord.TryParse(parts[0], out var header, out var error))
                {
                    adifFile = null;
                    reason = error;
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
                            dataLength = int.Parse(fieldLengthBuilder.ToString().Split(':')[0]);//fieldLength may be followed by optional : DataType
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
                            cursor++;
                        }
                        continue;
                    }
                }

                reason = null;
                return true;
            }

            reason = $"There were {parts.Length} parts instead of 2";
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