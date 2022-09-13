using AutoLectureRecorder.Data.Exceptions;

namespace AutoLectureRecorder.Data.Models
{
    public class Schedule
    {
        private Dictionary<WeekDay, SortedSet<Lecture>> _lecturesByDay;

        public Schedule(Dictionary<WeekDay, SortedSet<Lecture>> lecturesByDay)
        {
            _lecturesByDay = lecturesByDay;
        }

        private void CheckLectureOverlapInDay(WeekDay day, Lecture lecture)
        {
            if (_lecturesByDay.ContainsKey(day))
            {
                foreach (var existingLecture in _lecturesByDay[day])
                {
                    if (lecture.StartTime <= existingLecture.EndTime && existingLecture.StartTime <= lecture.EndTime)
                    {
                        throw new TimesOverlapException();
                    }
                }                
            }
        }

        /// <summary>
        /// Adds the given lecture to the list of the given day.
        /// The lectures of each day are sorted by start time in ascending order
        /// </summary>
        /// <param name="day"> The week day </param>
        /// <param name="lecture"> The lecture </param>
        public void AddLectureToDay(WeekDay day, Lecture lecture)
        {
            CheckLectureOverlapInDay(day, lecture);

            if (_lecturesByDay.ContainsKey(day))
            {
                _lecturesByDay[day].Add(lecture);
                return;
            }
            
            _lecturesByDay.Add(day, new SortedSet<Lecture>());
            _lecturesByDay[day].Add(lecture);
        }

        /// <summary>
        /// Gets a Sorted Set of the lectures in the given day in ascending order
        /// </summary>
        /// <param name="day"> The target day </param>
        /// <returns></returns>
        public SortedSet<Lecture> GetLecturesByDay(WeekDay day)
        {
            if (_lecturesByDay.ContainsKey(day))
            {
                return _lecturesByDay[day];
            }

            return null;
        }
    }
}