using System;

namespace DatabaseService.Dtos
{
    public class TemperaturDto
    {
        public DateTime Dato { get; set; }
        public TimeSpan Tidspunkt { get; set; }
        public decimal Grader { get; set; }
    }
}
