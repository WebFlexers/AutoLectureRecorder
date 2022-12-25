﻿namespace AutoLectureRecorder.Data.Models;

public class RecordedLecture
{
    public int Id { get; set; }
    public string StudentRegistrationNumber { get; set; }
    public string SubjectName { get; set; }
    public int Semester { get; set; }
    public string CloudLink { get; set; }
    public double StartedAt { get; set; }
    public double EndedAt { get; set; }
}