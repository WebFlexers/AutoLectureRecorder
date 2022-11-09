using Domain.Common;
using Domain.Exceptions;

namespace Domain.Entities;

public class WeekSchedule : BaseAuditableEntity
{
    private Dictionary<DayOfWeek, SortedSet<Lecture>> _lecturesSchedule;

    private void CheckLectureOverlapInDay(DayOfWeek day, Lecture lecture)
    {
        if (_lecturesSchedule.ContainsKey(day))
        {
            foreach (var existingLecture in _lecturesSchedule[day])
            {
                if (lecture.StartTime <= existingLecture.EndTime && existingLecture.StartTime <= lecture.EndTime)
                {
                    throw new TimesOverlapException("The times of lectures can't overlap");
                }
            }
        }
    }

    #region Public Methods
    /// <summary>
    /// Adds the given lecture to the list of the given day.
    /// The lectures of each day are sorted by start time in ascending order
    /// </summary>
    /// <param name="day"> The week day </param>
    /// <param name="lecture"> The lecture </param>
    public void AddLectureToDay(DayOfWeek day, Lecture lecture)
    {
        CheckLectureOverlapInDay(day, lecture);

        if (_lecturesSchedule.ContainsKey(day))
        {
            _lecturesSchedule[day].Add(lecture);
        }
        else
        {
            _lecturesSchedule.Add(day, new SortedSet<Lecture>());
            _lecturesSchedule[day].Add(lecture);
        }

        LastModified = DateTime.Now;
    }

    /// <summary>
    /// Removes the given lecture from the sorted set if it exists
    /// </summary>
    /// <param name="lecture"></param>
    public void RemoveLecture(Lecture lecture)
    {
        if (_lecturesSchedule.ContainsKey(lecture.DayOfWeek))
        {
            _lecturesSchedule[lecture.DayOfWeek].Remove(lecture);
        }
    }

    /// <summary>
    /// Gets a Sorted Set of the lectures in the given day in ascending order
    /// </summary>
    /// <param name="day"> The target day </param>
    /// <returns></returns>
    public SortedSet<Lecture> GetLecturesByDay(DayOfWeek day)
    {
        if (_lecturesSchedule.ContainsKey(day))
        {
            return _lecturesSchedule[day];
        }

        return null;
    }
    #endregion

    #region Constructors

    public WeekSchedule()
	{
        _lecturesSchedule = new Dictionary<DayOfWeek, SortedSet<Lecture>>();
		Created = DateTime.Now;
		LastModified = Created;
	}

    public WeekSchedule(Dictionary<DayOfWeek, SortedSet<Lecture>> schedule, DateTime created, DateTime lastModified)
    {
        _lecturesSchedule = schedule;
        Created = created;
        LastModified = lastModified;
    }

    #endregion
}
