namespace JsonSample
{
    public class UbikeOpenDatas
    {
        public bool success { get; set; }
        public DataResult result { get; set; }
    }

    public class DataResult
    {
        public string resource_id { get; set; }
        public int limit { get; set; }
        public int total { get; set; }
        public FieldType[] fields { get; set; }
        public BikeRecord[] records { get; set; }
    }

    public class FieldType
    {
        public string type { get; set; }
        public string id { get; set; }
    }

    public class BikeRecord
    {
        public string sno { get; set; }
        public string sna { get; set; }
        public string tot { get; set; }
        public string sbi { get; set; }
        public string sarea { get; set; }
        public string mday { get; set; }
        public string lat { get; set; }
        public string lng { get; set; }
        public string ar { get; set; }
        public string sareaen { get; set; }
        public string snaen { get; set; }
        public string aren { get; set; }
        public string bemp { get; set; }
        public string act { get; set; }
    }
}