# Battle of the Centerländ - TEAM 21 - AI

> **Team 21** entry for the **Sopra23 AI Contest** at the **University of Ulm**

**SUPER PLAYER** is written in **C#** and runs via [Docker](https://www.docker.com/).

The Docker image can be found in the [GitLab Container Registry](https://docs.gitlab.com/ee/user/packages/container_registry/).

## Basic Functionalities

- **Search Tree-Based Decision Making**: Uses advanced search tree algorithms to evaluate game states and determine optimal moves
- **Automated Game Playing**: Connects to game servers and plays autonomously without human intervention
- **Reconnection Handling**: Automatically attempts to reconnect after connection loss
- **Configurable Parameters**: Supports custom naming, connection settings, and delay configuration
- **Multi-Instance Support**: Run multiple AI instances simultaneously with automatic naming

## Installation

This is a private repository, so you need to log into the GitLab Registry.

**IMPORTANT:** This only works from the University network or via the University VPN!

```bash
docker login gitlab.uni-ulm.de:5050
```

Pull the Docker container with:

```bash
docker pull gitlab.uni-ulm.de:5050/softwaregrundprojekt/2022-2023/ki-turnier-server
```

## Usage

Start the Docker container with:

```bash
docker run --rm -it --network host -e IP=[TARGET-IP] -e PORT=[TARGET-PORT] -e NAME=AI_SUPER_PLAYER -e DELAY=3000 gitlab.uni-ulm.de:5050/softwaregrundprojekt/2022-2023/gruppenprojekt/group-21/ki:latest
```

For "TARGET-IP" and "TARGET-PORT", enter the IP address and port the AI should connect to.

With "NAME" you can give the AI a custom name. The number "1" will be appended to it. This value increases with each additional AI instance.

## Facts About the AI

- On connection loss, the AI initiates three reconnect attempts:

  [1] After 5 seconds

  [2] After 15 seconds

  [3] After 30 seconds

- Test coverage of up to 100%
- Developed by computer science experts
- Optimized through months of research
- Verified with Proof of Big Data
- Watch out or the AI will beat you!
