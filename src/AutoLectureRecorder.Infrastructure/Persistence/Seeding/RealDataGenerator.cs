using AutoLectureRecorder.Domain.SqliteModels;

namespace AutoLectureRecorder.Infrastructure.Persistence.Seeding;

/// <summary>
/// This class is used to generate real existing lectures for demonstration purposes
/// </summary>
public static class RealDataGenerator
{
    public static List<ScheduledLecture> GenerateLecturesForSemester(int semester)
    {
        switch (semester)
        {
            case 7:
                return GenerateLecturesForSemester7();
            case 8:
                return GenerateLecturesForSemester8();
            default:
                throw new ArgumentException("There is no real data to generate for the given semester",
                    nameof(semester));
        }
    }

    private static List<ScheduledLecture> GenerateLecturesForSemester7()
    {
        List<ScheduledLecture> lectures = new()
        {
            new ScheduledLecture(1, "Ανάλυση Εικόνας", 7, "teams.microsoft.com", 3, 
                "12:15 pm", "02:00 pm", 0, 1),
            new ScheduledLecture(2, "Ανάλυση Εικόνας", 7, "teams.microsoft.com", 4, 
                "02:15 pm", "04:00 pm", 0, 0),
            
            new ScheduledLecture(3, "Εικονική Πραγματικότητα", 7, "teams.microsoft.com", 1, 
                "04:15 pm", "06:00 pm", 0, 1),
            new ScheduledLecture(4, "Εικονική Πραγματικότητα", 7, "teams.microsoft.com", 5, 
                
                "02:15 pm", "04:00 pm", 1, 0),
            new ScheduledLecture(5, "Σύγχρονα Θέματα Τεχνολογίας Λογισμικού - Λογισμικό για Κινητές Συσκευές", 
                7, "teams.microsoft.com", 1, "02:15 pm", "04:00 pm", 0, 0),
            new ScheduledLecture(6, "Σύγχρονα Θέματα Τεχνολογίας Λογισμικού - Λογισμικό για Κινητές Συσκευές",
                7, "teams.microsoft.com", 5, "10:15", "12:00 pm", 0, 1),
            
            new ScheduledLecture(7, "Τεχνολογίες Ανάπτυξης Ηλεκτρονικών Παιχνιδιών",
                7, "teams.microsoft.com", 3, "09:15", "12:00 pm", 0, 0),
            new ScheduledLecture(8, "Τεχνολογίες Ανάπτυξης Ηλεκτρονικών Παιχνιδιών",
                7, "teams.microsoft.com", 5, "10:15", "11:00", 0, 1),
            
            new ScheduledLecture(9, "Υπηρεσιοστρεφές Λογισμικό",
                7, "teams.microsoft.com", 3, "10:15", "11:00", 0, 1),
            new ScheduledLecture(10, "Υπηρεσιοστρεφές Λογισμικό",
                7, "teams.microsoft.com", 4, "10:15", "11:00", 0, 1)
        };

        return lectures;
    }
    
    private static List<ScheduledLecture> GenerateLecturesForSemester8()
    {
        List<ScheduledLecture> lectures = new()
        {
            new ScheduledLecture(11, "Εκπαιδευτικό Λογισμικό", 8, "teams.microsoft.com", 4, 
                "02:15 pm", "04:00 pm", 1, 1),
            new ScheduledLecture(12, "Εκπαιδευτικό Λογισμικό", 8, "teams.microsoft.com", 5, 
                "10:15", "12:00 pm", 1, 0),
            
            new ScheduledLecture(13, "Επεξεργασία Φωνής και Ήχου", 8, "teams.microsoft.com", 5, 
                "12:15 pm", "02:00 pm", 1, 1),
            new ScheduledLecture(14, "Επεξεργασία Φωνής και Ήχου", 8, "teams.microsoft.com", 5, 
                "02:15 pm", "4:00 pm", 1, 0),
            
            new ScheduledLecture(15, "Ευφυείς Πράκτορες", 8, "teams.microsoft.com", 2, 
                "02:15 pm", "05:00 pm", 1, 0),
            
            new ScheduledLecture(16, "Συστήματα ERP/CRM", 8, "teams.microsoft.com", 1, 
                "01:15 pm", "02:00 pm", 1, 0),
            new ScheduledLecture(17, "Συστήματα ERP/CRM", 8, "teams.microsoft.com", 4, 
                "04:15 pm", "06:00 pm", 1, 1),
            
            new ScheduledLecture(18, "Τεχνολογίες Blockchain και Εφαρμογές", 8, "teams.microsoft.com", 3, 
                "09:15", "11:00", 1, 0),
            new ScheduledLecture(19, "Τεχνολογίες Blockchain και Εφαρμογές", 8, "teams.microsoft.com", 5, 
                "04:15 pm", "06:00 pm", 1, 1),
        };
        
        return lectures;
    }
}