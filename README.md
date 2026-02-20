# Battle of the Centerländ - TEAM 21 - KI 

**SUPER PLAYER** ist in **C#** geschrieben und wird via [Docker](https://www.docker.com/) gestartet.

Der Docker kann im [GitLab Container Registry](https://docs.gitlab.com/ee/user/packages/container_registry/) gefunden werden.

## Installation

Dies ist ein Privates Repo, deshalb musst du dich ins das GitLab Registry einloggen.

WICHTIG: Das funktioniert nur vom Uni Netzwerk oder dem Uni VPN!

```bash
docker login gitlab.uni-ulm.de:5050
```

Pull den Docker container mit:

```bash
docker pull gitlab.uni-ulm.de:5050/softwaregrundprojekt/2022-2023/ki-turnier-server
```

## Benutzung

Starte den Docker mit:

```bash
docker run --rm -it --network host -e IP=[ZIEL-IP] -e PORT=[ZIEL-PORT] -e NAME=AI_SUPER_PLAYER -e DELAY=3000 gitlab.uni-ulm.de:5050/softwaregrundprojekt/2022-2023/gruppenprojekt/group-21/ki:latest
```
Gib bei "ZIEL-IP" und "ZIEL-Port" bitte die IP und Port ein, mit der die AI sich verbinden soll.

Bei "NAME" kannst du der AI einen eigenen Namen geben. Dieser bekommt die Nummer "1" angehängt. Dieser Wert wird mit jeder weiteren AI erhöht.

## Fakten über die AI

- Bei einem Verbindungsabbruch startet die AI drei Reconnect-Versuche:

[1] Nach  5 Sekunden

[2] Nach 15 Sekunden

[3] Nach 30 Sekunden

- Testabdeckung von bis zu 100%
- Von Informatik-Experten entwickelt.
- Durch monatelange Forschung optimiert.
- Mit Proof of Big Data verifiziert.
- Pass auf oder die AI besiegt dich.

## Lizenz

[STUDIO 21](https://www.youtube.com/watch?v=aRsWk4JZa5k)