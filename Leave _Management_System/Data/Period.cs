namespace Leave__Management_System.Data
{
    public class Period: BaseEntity
    {
     
        public string Name { get; set; } = string.Empty;
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
    }
}
