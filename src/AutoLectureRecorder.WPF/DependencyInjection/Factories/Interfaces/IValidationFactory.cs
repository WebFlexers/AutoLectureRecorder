using System.Collections.Generic;
using AutoLectureRecorder.Data.ReactiveModels;
using FluentValidation;

namespace AutoLectureRecorder.DependencyInjection.Factories.Interfaces;

public interface IValidationFactory
{
    IValidator<ReactiveScheduledLecture> CreateReactiveScheduledLectureValidator(List<ReactiveScheduledLecture>? existingLectures = null);
}