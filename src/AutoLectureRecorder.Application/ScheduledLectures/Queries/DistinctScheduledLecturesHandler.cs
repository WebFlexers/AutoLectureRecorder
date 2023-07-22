using System.Collections.ObjectModel;
using AutoLectureRecorder.Application.Common.Abstractions.Persistence;
using AutoLectureRecorder.Domain.ReactiveModels;
using MediatR;

namespace AutoLectureRecorder.Application.ScheduledLectures.Queries;

public class DistinctScheduledLecturesHandler 
    : IRequestHandler<DistinctScheduledLecturesQuery, ObservableCollection<ReactiveScheduledLecture>>
{
    private readonly IScheduledLectureRepository _scheduledLectureRepository;

    public DistinctScheduledLecturesHandler(IScheduledLectureRepository scheduledLectureRepository)
    {
        _scheduledLectureRepository = scheduledLectureRepository;
    }
    
    public async Task<ObservableCollection<ReactiveScheduledLecture>> Handle(DistinctScheduledLecturesQuery request, 
        CancellationToken cancellationToken)
    {
        var lecturesWithDistinctNames = await _scheduledLectureRepository
            .GetScheduledLecturesGroupedByName();

        if (lecturesWithDistinctNames is not null && lecturesWithDistinctNames.Any())
        {
            return new ObservableCollection<ReactiveScheduledLecture>(lecturesWithDistinctNames);
        }
        else
        {
            return new ObservableCollection<ReactiveScheduledLecture>();
        }
    }
}