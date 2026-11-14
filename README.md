# Forest Siege
Unity 2022.3 LTS / WebGL. Desktop prototype, not a release build.

## Run
1. Install Unity 2022.3.62f1 with WebGL Build Support via Unity Hub.
2. Open this folder as a project. Ensure Active Input Handling is Input Manager (Old) or Both.
3. Select Forest Siege > Create Scene, then Play.
4. Forest Siege > Build WebGL creates Build/WebGL. Serve via HTTP locally; do not open index.html via file://.

WASD, mouse, Shift, Space, 1–3 weapons, R reload, right mouse aim, Esc pause. Fourteen bots, defeat/victory and restart. Map 1609.344 m square, two ruined forts, sparse forest and two dense patches. Primitive graphics and simple AI; no navigation mesh. No multiplayer implementation yet. No real weapon assets or sound effects. Unity compilation and browser performance have not been tested.

## Yandex Games
The WebGL bridge loads /sdk.js and calls LoadingAPI.ready after generation. Verify SDK initialization, focus handling, keyboard layouts, performance and moderation requirements before publication. ZIP the contents of Build/WebGL (index.html at root), not the source directory. Uncompressed limit 100 MB. Desktop only for now.

## Multiplayer plan
Use an authoritative dedicated server with secure WebSocket transport compatible with WebGL, guest sessions, server-side movement and shot validation, snapshots and interpolation. Do not trust client-provided damage or positions. No live server is included.

## GitHub
Create an empty repository named forest-siege, then run from this directory:
```
git init -b main
git add .
git commit -m "Initial Unity FPS prototype"
git remote add origin https://github.com/YOUR_ACCOUNT/forest-siege.git
git push -u origin main
```
