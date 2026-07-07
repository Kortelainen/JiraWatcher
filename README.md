# JiraWatcher

![JiraWatcherIcon](https://github.com/Kortelainen/JiraWatcher/assets/10597651/b1a47111-82bc-4821-bcfd-4e1cdcd9beed)

## Description
JiraWatcher is a straightforward desktop application designed to streamline issue tracking within Jira. It leverages JQL in the Jira API to fetch a dashboard-like list of items based on user-defined search criteria. The primary objective of this project is to offer a configurable and visually appealing alternative to the standard Jira dashboard. The project was created to help my personal need to keep track of certain tickets types.

### Key Features:
- Automatic dashboard refresh every minute.
- Notification alerts and taskbar flashing for newly added items.
- Local storage of all settings ensures security and convenience.

<img width="985" height="522" alt="image" src="https://github.com/user-attachments/assets/4c496847-7492-43c3-b3cb-3f21a62137d3" />


## Requirements
- Windows 11 64-bit (Other platforms remain untested)
- Jira installation with Jira API access (For cloud create token [here](https://id.atlassian.com/manage-profile/security/api-tokens))
- Jira API version 3 support only (Latest jira cloud)

## Setup
Download release zip from git sidebar and extact to preferred location. Create windows shortcuts if preferred.

### Settings:
- **Jira URL:** Provide the basic Jira URL (e.g., `https://customer.atlassian.net/`). Used for redirection when clicking on tickets.
- **Jira API URL:** Enter the API URL, typically `https://customer.atlassian.net/rest/api` for cloud users.
- **Jira API Username / Password:** Mandatory for authentication.
- **JQL:** Construct your query using the Jira search feature, then copy and paste it here. Basic validation is in place, though further enhancements are planned.

Application will display example tickets on a list until settings are properly configured. Here is what the settings page looks like right now.
<img width="739" height="610" alt="image" src="https://github.com/user-attachments/assets/06b0b948-8bc4-47f0-a0d8-e58b9908ed6b" />

## Future Roadmap
Here are the planned features and fixes, listed in no particular order:
- UX/UI Enhancements
- Implement more robust error handling mechanisms.
- Integrate with Windows native notifications for a seamless user experience.
- Provide a visual indicator for an empty Jira item list.
- Enhance configurability of refresh rates while considering Jira API throttling limitations.
- Sign the exe with https://signpath.org/
- Main dashboard with graph and visualization tools
- Mass update gigs trough configured workflows
- Configure dashboard columns
- Peek at a gig
- Modify/update gig.

See [CHANGELOG.md](CHANGELOG.md) for version history.
