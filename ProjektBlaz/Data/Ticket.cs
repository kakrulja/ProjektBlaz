namespace ProjektBlaz.Data
{
    public class Ticket
    {
        public int Id { get; set; }
        public string SenderEmail { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public DateTime ReceivedAt { get; set; } = DateTime.UtcNow;
        public bool IsResolved { get; set; } = false;
    }

}
