using System.Collections.ObjectModel;
using AutoLectureRecorder.Domain.ReactiveModels;
using MediatR;

namespace AutoLectureRecorder.Application.ScheduledLectures.Queries;

public record DistinctScheduledLecturesQuery() : IRequest<ObservableCollection<ReactiveScheduledLecture>>;