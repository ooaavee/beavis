namespace Beavis.Isolation.Contracts
{
    public class ModuleResponse
    {
        /// <summary>
        /// TÄMÄ LÄHTEE POIS!!!
        /// </summary>
        public string Data { get; set; }

        public HttpResponseEnvelope Content { get; set; } = new HttpResponseEnvelope();
    }
}
