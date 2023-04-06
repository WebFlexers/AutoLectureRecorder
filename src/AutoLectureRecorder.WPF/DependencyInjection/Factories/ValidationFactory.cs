using System.Collections.Generic;
using AutoLectureRecorder.Data.ReactiveModels;
using AutoLectureRecorder.DependencyInjection.Factories.Interfaces;
using AutoLectureRecorder.Services.DataAccess.Validation;
using FluentValidation;

namespace AutoLectureRecorder.DependencyInjection.Factories;

public class ValidationFactory : IValidationFactory
{
    public IValidator<ReactiveScheduledLecture> CreateReactiveScheduledLectureValidator(List<ReactiveScheduledLecture>? existingLectures = null)
    {
        return new ReactiveScheduledLectureValidator(existingLectures);
    }
}
