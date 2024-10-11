# AMWE-Server
C# .NET Assistant in Monitoring the Work of Employees Server  
Project page: http://amwe.glitch.me

---

## What is this?
Assistant in Monitoring the Work of Employees (AMWE) is a project created to help monitor employees working remotely and in the office. It was developed in connection with the COVID-19 pandemic.

This project awarded 50.000₽ in MIPT PhysTech Lycieum Start to Innovate 2022 conference as best Tech Startup Idea.

Project capabilities:

- Automation: Monitor incoming reports from all user computers in the network
- Summaries: Monitor the dynamics of each employee's work in the network
- Analytics: Automatic processing of information in reports using a neural network configured to your requirements
- Real Time monitoring: Transfering of all data in real time
- Evaluation: Custom evaluation criteria to simplify monitoring of employee work
- Screenshots: The ability to obtain screenshots for independent analysis of each individual computer
- Flexibility: Customizable lists of prohibited programs and sites for software processing
- Simplified design: There is nothing superfluous in it. Everything for fast and convenient work
- Performance: The program is not demanding on hardware
- Continuity: The application on the employee side works absolutely in the background
- Security: Authentication system, logging of actions for the administrator

## Full list of features

- Open a workday. Clients alrady connected to your network and waiting for start working!
- Collect reports genreated by clients, which contains next information:
  - Keyboard & Mouse activity
  - Process list
  - Current opened tab (works in majority of popular browsers)
  - Neural Network judgement result about PC usage according to information above
- There're additional autocheck for restricted processes and tabs in administrator panel
  - Process and website list may be setup manually
- All of potentially inactive clients (judged above 0.5) marked with red color for it better searching
- All of the reports collecting in separate tab with graphical information about report marks
- Ability to get webcam image & screenshot from user (and save it on disk)
- Open a instant messaging chat with any user
  - If user close chat connection, administrator will see user ID in close message (in order to know with whom you need to open a chat again)
- Flexible settings, 4 color themes
- Close workday, clients will stop sending reports and will be disconnected from server until next morning

Here's a large demo of this features:

https://github.com/user-attachments/assets/d3340799-e103-456f-a850-620901a9ba31

## How to run?

Download the binaries of [client](https://github.com/plmlkff/HttpClientWpf/releases/tag/v1.3.200222) and administrator (see Releases). Unfortunately, without rebuilding client project it is impossible to switch server address, so, here is demo network credentials:
- Server address: http://amwe-server.glitch.me/
- Server password: Contact Telegram @zerumi
