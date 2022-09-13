#! /bin/bash
cd ~/Documents/Federico/AI_Stoplight-master/venv && source bin/activate && cd ~/Documents/Federico/AI_Stoplight-master/venv && mlagents-learn Config/Config.yaml --resume --env=One_Lane_Violino_Build/AI_Stoplights --run-id=One_Lane_Violino --width=720 --height=480 --num-envs 4 --quality-level=0
