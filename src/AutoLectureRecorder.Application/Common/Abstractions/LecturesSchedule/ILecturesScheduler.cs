using AutoLectureRecorder.Domain.ReactiveModels;
using ReactiveUI;

namespace AutoLectureRecorder.Application.Common.Abstractions.LecturesSchedule;

public interface ILecturesScheduler : IReactiveObject
{
    ReactiveScheduledLecture? NextScheduledLecture { get; set; }
    TimeSpan? NextScheduledLectureTimeDistance { get; }
    IObservable<bool> NextScheduledLectureWillBegin { get; }
}