# Unhackable Vote App

This app is a very simple vote app web site for Unhackable Inc.

> Note: This application is for demonstration purposes only

> :warning: ** The vote web app has a web and api interface that allows running arbitrary commands on the host/container

## Changes made to original work

This repo is based on the [Example Voting App](https://github.com/dockersamples/example-voting-app).

Major change listed here:
* VoteApp now rates shoes instead of pets
* VoteApp was changed to include 'hidden' hacker interface that allow for executing Python commands from the UI and API
* Works has been ported to new binary that run on Windows on host instead of in a container
