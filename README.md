# JiraWatcher
![JiraWatcherIcon](https://github.com/Kortelainen/JiraWatcher/assets/10597651/9ebb3786-5089-4b4b-9cda-c6edd8944ea7)


## Description
Simple desktop app to track jira issues. This applicaiton uses JQL in jira api to get dashboard like item list to show searched items. Main goal of this project is to provide configurable and styled alternative to jira dashboard. The project was created to help my personal need to keep track of certain items in timely manner. I find using Jira web-app frustrating.

* Dashboard is refreshed every minute.
* New items in list cause notification sound and taskbar flash


![MainWindow](https://github.com/Kortelainen/JiraWatcher/assets/10597651/9eb54356-44fd-416b-b04b-aff36e0ebba9)



## Requirements
* Windows 11 (Soft requirement, other platforms are untested)
* Jira + Jira API
* Jira api version 3 suport only

## Setup
* Jira url: This should be basic jira url. ie https://customer.atlassian.net/
  * Used for the redirection when ticket is clicked.
* Jira API URL: API url, for cloud users its something like this. https://customer.atlassian.net/rest/api
* Jira api username / password. Necessary
* JQL. Jira querry language for the task list. It's recomended you go to jira search feature and costruct your JQL querry in there and copy paste it into this box. For now there is only barebones validation on my app.
![SettingsWindow](https://github.com/Kortelainen/JiraWatcher/assets/10597651/6e15e8e0-a5a8-4ffa-9e03-62cf1d91359e)

## Future roadmap
Planned features and fixes in no particular order
* Fix error icon to be hidden when no error occurs. The icon should display exception messages to user for troubleshooting
* More robust error handling
* Allow user to enable or disable notifications and their sounds
* Use windows integrated notifications
* Empty jira item list splash art
* Styling fixes
* Style sheets such as dark and light mode.
* Multiple tabs each with own JQL + Tab spesific settings.

