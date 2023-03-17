using System.Collections.Generic;
using AutoLectureRecorder.Data.ReactiveModels;
using AutoLectureRecorder.Services.DataAccess.Validation;
using FluentValidation;

namespace AutoLectureRecorder.DependencyInjection.Factories;

public class ValidatorFactory : IValidationFactory
{
    public IValidator<ReactiveScheduledLecture> CreateReactiveScheduledLectureValidator(List<ReactiveScheduledLecture>? existingLectures = null)
    {
        return new ReactiveScheduledLectureValidator(existingLectures);
    }
}
