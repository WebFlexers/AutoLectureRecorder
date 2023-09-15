<img src="https://github.com/WebFlexers/AutoLectureRecorder/assets/57811193/182499ed-8759-4ab7-a2b5-717a4271d666" width="200" height="200"/>

# What is Auto Lecture Recorder?
Auto Lecture Recorder is a WPF application that enables students of University of Piraeus to create a **schedule** of their lectures and **automatically join** them on Microsoft Teams and **record them** for watching at a later time.
This application was created during the pandemic to give back to students what is rightfully theirs. **Their sacred sleep time.** Auto Lecture Recorder acted as the **candle** that kept showing the students 
the **light** in the **dark times of the pandemic**.

</br>

# How to set it up?
Setting up Auto Lecture Recorder is pretty simple

1) [Download the **latest release**](https://github.com/WebFlexers/AutoLectureRecorder/releases/download/v3.0.0/AutoLectureRecorder.rar) from this repository
2) **Extract** to the directory of your choice
3) (Optional) Create a desktop shortcut by right clicking AutoLectureRecorder.exe and clicking Send To -> Desktop
4) **Open Microsoft Edge** in order for it to update automatically (this is necessary, because Auto Lecture Recorder relies on Microsoft Edge behind the scenes to automatically join microsoft teams meetings)
5) **Disable** the **auto sleep** and **auto turn off of your computer screen** through the Windows settings, because this can interfere with the recording process or lead to a failure to join the meeting in the first place
6) Click on AutoLectureRecorder.exe to run the app
7) (Optional) If you want to record lectures while you sleep you can enable the Launch on Startup option in the settings page of Auto Lecture Recorder in order to start it automatically with windows (more information on the Settings Page section of this documentation).
In addition to this you can modify your computer's bios in order to automatically boot the pc at a certain time. Keep in mind that every motherboard is different so you will have to find out how to do this on your own. This way the process will be 100% automatic.

</br>

# How does it work?

## Logging in

When you first launch the app you will be greeted with the following login screen:

![login_light_empty](https://github.com/WebFlexers/AutoLectureRecorder/assets/57811193/96d60117-53de-4e9a-8191-1b87a094665f)

</br>

All you have to do is fill in your academic information. That is your academic email and your password. For example:

![login_light_filled](https://github.com/WebFlexers/AutoLectureRecorder/assets/57811193/82136c03-4854-4992-a9a2-8bef12e9f2d7)

</br>

After that the login process will commence. Just wait until it's finished and you land on the first page of the app

![sign_in_light](https://github.com/WebFlexers/AutoLectureRecorder/assets/57811193/f16c985e-9885-4bb3-90f7-c6dee72a0c43)

</br>

## Dashboard Page

This is the main page of the app. On the left side you can see which lecture is scheduled to be recorded next, when it will start and at the bottom of the page some statistics
about the lectures. On the right hand side there are all the lectures that are scheduled for today. The first time you open the app all of these will be empty, since you 
have not yet scheduled your lectures. If that is the case you can click the Schedule Lectures button on the right to navigate to a page where you can schedule your lectures

![dashboard_light_empty](https://github.com/WebFlexers/AutoLectureRecorder/assets/57811193/50086711-c6e7-4f27-a97a-b0e904103963)

</br>

## Schedule Lectures Page

In order to schedule lectures you need to click the Scheduled Lectures button either on the Dashboard page or on the Schedule page.
Then you will get to the following page:

![create_lecture_light_empty](https://github.com/WebFlexers/AutoLectureRecorder/assets/57811193/e684b0c7-0aca-437e-b272-61017cdd7b69)

To schedule a lecture you need to fill in the following information:
1) **The subject name.** This can be anything you want, but you are advised to use the name of the subject you want to record
2) **The semester** of the lecture
3) **The meeting link.** This should be the link that you normally use to enter the Microsoft Teams meeting. For now only direct meeting links work. If you try to use a link that points to the Team instead of the meeting it will not work.
4) **The day** that the lecture starts
5) **The start time** of the lecture
6) **The end time** of the lecture
7) Whether or not to automatically **upload** to the cloud (as of now this feature is not available yet, so it doesn't matter what you choose)
8) Whether or not the lecture **will be scheduled** for recording. This needs to be checked if you want to automatically join and record the lecture

Once you are done click on the Create Lecture button. Here is an example of the filling process:

![create_lecture_light_filled](https://github.com/WebFlexers/AutoLectureRecorder/assets/57811193/2036c944-c307-43ed-966a-4fc56c8966d1)

## Schedule Page

In this page you can see and manage all your scheduled lectures. Specifically you can
1) **Filter** the lectures by subject name, semester, active status (whether they are scheduled or not) and/or upload status.
2) Select 1 or more lectures in order to **delete** them
3) Change the **active and/or upload status** of individual lectures by adjusting the checkboxes
4) **Edit** individual lectures by clicking the cogwheel icon


Here is an example of the schedule page with the lecture we added previously:

![schedule_light_one_lecture](https://github.com/WebFlexers/AutoLectureRecorder/assets/57811193/36bd3199-0450-464c-ba92-e34ef2750d39)

And here is an example of a schedule with more lectures that are filtered by semester and active status:

![schedule_light_filled_filtered_by_semester8_and_active](https://github.com/WebFlexers/AutoLectureRecorder/assets/57811193/49c0739d-d52f-4148-adfa-7eae54ce58e2)

## Upload Page

This page is not yet complete and therefore the automatic upload feature is not currently available

## Library Page

In this page you can find the recorded lectures and watch them on your favourite player. The first screen is:

![library_light_filled](https://github.com/WebFlexers/AutoLectureRecorder/assets/57811193/26b3eda8-2cbb-4c30-9282-896689b83b78)

Once you click on a subject you can see all the recorded lectures so far

![library_recorded_lectures](https://github.com/WebFlexers/AutoLectureRecorder/assets/57811193/00be6304-811c-4c36-96c8-778fd74dc2e1)

You can click the Watch Locally button to watch the lecture in your favourite player.

## Settings

In this page you can alter several settings for the app. In particular there are 2 categories of settings. 
The first on is general settings where you can:

1) Decide whether or not you want the app to start with Windows. This is important if you want to record a lecture while you are sleeping. You can set your computer to automatically
boot at a certain time (this can usually be done through the bios) and have Auto Lecture Recorder start automatically. Keep in mind that for this to work there needs to be only 1
windows user and it needs to be passwordless so it logs in automatically. If you want to avoid all this you can also just keep the computer on
2) Decide whether or not you want the app to minimize instead of completely shutting down when clicking the x button. If this option is enabled you have to right click the tray icon
and choose Exit in order for the app to shut down completely

![settings_general](https://github.com/WebFlexers/AutoLectureRecorder/assets/57811193/084addc2-45c6-469c-aeda-fcfa04cc1ec2)

The second category is recording settings where you can alter several settings regarding the recording process. It looks like so:

![settings_recording](https://github.com/WebFlexers/AutoLectureRecorder/assets/57811193/5288a7c2-30f4-4c34-acf5-fdbaeb336060)

# Dark mode

For those of us with sensitive eyes there is also a dark mode, that you can enable by clicking the toggle button next to the window buttons in the top right corner.
The dark mode looks like this:

## Login

![Login](https://github.com/WebFlexers/AutoLectureRecorder/assets/57811193/cd5aa373-ca84-47cb-8ec7-11fdb14c82e9)

## Dashboard

![Dashboard](https://github.com/WebFlexers/AutoLectureRecorder/assets/57811193/12f30b8b-72d7-447d-b063-ceecc91b84c7)

## Create Lectures

![Create Lectures](https://github.com/WebFlexers/AutoLectureRecorder/assets/57811193/1a50026f-e93b-407b-80f3-5261787b12f3)

## Schedule

![Schedule](https://github.com/WebFlexers/AutoLectureRecorder/assets/57811193/ea381759-a4ae-457a-92e0-0e02d8005182)

## Library

![Library](https://github.com/WebFlexers/AutoLectureRecorder/assets/57811193/f2e06c22-6ada-424a-a62c-63fc94219447)

## Settings

![Settings](https://github.com/WebFlexers/AutoLectureRecorder/assets/57811193/b48e2342-6bbb-4828-8bc9-d6f0660a7a52)
