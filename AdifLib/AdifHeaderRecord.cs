namespace M0LTE.AdifLib
{
    public class AdifHeaderRecord : AdifRecord
    {
        protected override string EndMarker => "<eoh>";

        public string AdifVersion
        {
            get => Fields.TryGetValue("adif_ver", out string value) ? value : null;
            set => SetField("adif_ver", value);
        }

        public string ProgramId
        {
            get => Fields.TryGetValue("programid", out string value) ? value : null;
            set => SetField("programid", value);
        }

        public static bool TryParse(string record, out AdifHeaderRecord adifHeader, out string error)
        {
            if (AdifRecord.TryParse(record, out var adifRecord, out error))
            {
                adifHeader = new AdifHeaderRecord();

                foreach (var kvp in adifRecord.Fields)
                {
                    adifHeader.Fields.Add(kvp.Key, kvp.Value);
                }

                error = null;
                return true;
            }

            adifHeader = null;
            return false;
        }
    }
}