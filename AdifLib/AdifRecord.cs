﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace AdifLib
{
    public class AdifRecord
    {
        public Dictionary<string, string> Fields { get; private set; } = new Dictionary<string, string>();

        public DateTime QsoStart
        {
            get
            {
                if (!Fields.TryGetValue("qso_date", out string qsoDate)) return default;
                if (!Fields.TryGetValue("time_on", out string timeOn)) return default;
                if (qsoDate.Length != 8) return default;
                if (timeOn.Length != 6) return default;

                if (!int.TryParse(qsoDate.Substring(0, 4), out int year)) return default;
                if (!int.TryParse(qsoDate.Substring(4, 2), out int month)) return default;
                if (!int.TryParse(qsoDate.Substring(6, 2), out int day)) return default;
                if (!int.TryParse(timeOn.Substring(0, 2), out int hour)) return default;
                if (!int.TryParse(timeOn.Substring(2, 2), out int min)) return default;
                if (!int.TryParse(timeOn.Substring(4, 2), out int sec)) return default;

                return new DateTime(year, month, day, hour, min, sec, DateTimeKind.Utc);
            }
        }

        public string Call
        {
            get => Fields.TryGetValue("call", out string value) ? value : null;
            set => SetField("call", value);
        }

        private void SetField(string fieldName, string value) => Fields[fieldName] = value;

        public override string ToString() => string.Join(" ", Fields.Select(f => $"<{f.Key}:{f.Value.Length}>{f.Value}")) + " <eor>";

        public static bool TryParse(string record, out AdifRecord adifRecord, out string error)
        {
            adifRecord = new AdifRecord();

            ParseState p = ParseState.LookingForStartOfRecord;
            var fieldNameBuffer = new List<char>();
            var fieldLenBuffer = new List<char>();
            var dataBuffer = new List<char>();

            try
            {
                for (int i = 0; i < record.Length; i++)
                {
                    if (p == ParseState.LookingForStartOfRecord)
                    {
                        if (record[i] == '<')
                        {
                            p = ParseState.FieldName;
                        }
                        else continue;
                    }
                    else if (p == ParseState.FieldName)
                    {
                        if (record[i] == ':')
                        {
                            p = ParseState.FieldLen;
                        }
                        else
                        {
                            fieldNameBuffer.Add(record[i]);
                        }
                    }
                    else if (p == ParseState.FieldLen)
                    {
                        if (record[i] == '>')
                        {
                            p = ParseState.Data;
                        }
                        else
                        {
                            fieldLenBuffer.Add(record[i]);
                        }
                    }
                    else if (p == ParseState.Data)
                    {
                        int fieldLen = int.Parse(new String(fieldLenBuffer.ToArray()));
                        if (dataBuffer.Count == fieldLen)
                        {
                            string fieldName = new string(fieldNameBuffer.ToArray());
                            string data = new string(dataBuffer.ToArray());

                            adifRecord.Fields.Add(fieldName, data);

                            fieldNameBuffer.Clear();
                            fieldLenBuffer.Clear();
                            dataBuffer.Clear();
                            p = ParseState.LookingForStartOfRecord;
                        }
                        else
                        {
                            dataBuffer.Add(record[i]);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return false;
            }

            error = null;
            return true;
        }

        private enum ParseState
        {
            LookingForStartOfRecord, FieldName, FieldLen, Data
        }
    }
}