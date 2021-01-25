namespace AAS.Architecture.Gateway.Cors
{
    public class CorsOptions
    {
        public bool Enabled { get; set; }
        public string[] Domains { get; set; }
        public string[] Methods { get; set; }
        public string[] Headers { get; set; }
        public string[] ExposedHeaders { get; set; }
    }
}