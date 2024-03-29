﻿using AutoLectureRecorder.Application.Login.Queries.LoginToMicrosoftTeams;
using AutoLectureRecorder.Application.UnitTests.TestUtils.Constants;

namespace AutoLectureRecorder.Application.UnitTests.Login.TestsUtils;

public static class LoginToMicrosoftTeamsQueryUtils
{
    public static LoginToMicrosoftTeamsQuery CreateQuery()
    {
        return new LoginToMicrosoftTeamsQuery(
            Constants.LoginToMicrosoftTeams.AcademicEmailAddress, 
            Constants.LoginToMicrosoftTeams.Password);
    }
}