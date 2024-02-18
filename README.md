# JiraWatcher

![JiraWatcherIcon](https://github.com/Kortelainen/JiraWatcher/assets/10597651/b1a47111-82bc-4821-bcfd-4e1cdcd9beed)

## Description
JiraWatcher is a straightforward desktop application designed to streamline issue tracking within Jira. It leverages JQL in the Jira API to fetch a dashboard-like list of items based on user-defined search criteria. The primary objective of this project is to offer a configurable and visually appealing alternative to the standard Jira dashboard. Originally conceived to meet personal needs for timely item tracking, JiraWatcher aims to alleviate the frustrations often encountered with the Jira web application.

### Key Features:
- Automatic dashboard refresh every minute.
- Notification alerts and taskbar flashing for newly added items.
- Local storage of all settings ensures security and convenience.

![MainWindow](https://github.com/Kortelainen/JiraWatcher/assets/10597651/d5d29b5f-eec6-44e2-a244-5a8f025f7b02)

## Requirements
- Windows 11 (Other platforms remain untested)
- Jira installation with Jira API access
- Jira API version 3 support only

## Setup
- **Jira URL:** Provide the basic Jira URL (e.g., `https://customer.atlassian.net/`). Used for redirection when clicking on tickets.
- **Jira API URL:** Enter the API URL, typically `https://customer.atlassian.net/rest/api` for cloud users.
- **Jira API Username / Password:** Mandatory for authentication.
- **JQL:** Construct your query using the Jira search feature, then copy and paste it here. Basic validation is in place, though further enhancements are planned.

![SettingsWindow](https://github.com/Kortelainen/JiraWatcher/assets/10597651/2e6a298b-4017-4c44-8053-350c41bf09ec)

## Future Roadmap
Here are the planned features and fixes, listed in no particular order:
- Hide error icon when no errors occur; display exception messages for troubleshooting.
- Implement more robust error handling mechanisms.
- Allow users to enable/disable notifications and customize notification sounds.
- Integrate with Windows native notifications for a seamless user experience.
- Provide a visual indicator for an empty Jira item list.
- Address styling inconsistencies and introduce style sheets such as dark and light mode.
- Implement multiple tabs, each with its own JQL query and tab-specific settings.
- Enhance configurability of refresh rates while considering Jira API throttling limitations.
