using NetMessage.Game;
using Network;
using UnityEngine;
using WX;

namespace Requester
{
    class GameRequester : BaseRequester<GamePack>
    {
        private const ActionType m_ActionType = ActionType.Game;

        public override void OnResponse(byte[] msg_body)
        {
            var pack = Decode(msg_body);
            var room = Client.instance.GetRoom;
            if (room==null)
            {
                Log.LogWarning($"未找到房间: RoomId is {pack.RoomId}");
                return;
            }
            switch (pack.FuncCode)
            {
                case FuncCode.Move:
                    room.gameManager.PlayerMove(pack.MovePack);
                    break;
                case FuncCode.Damage:
                    room.gameManager.PlayerDamgage(pack.DamagePack);
                    break;
                case FuncCode.NewBullet:
                    room.gameManager.NewBullet(pack.BulletPack);
                    break;
                case FuncCode.DelBullet:
                    room.gameManager.DelBullet(pack.BulletPack);
                    break;
                case FuncCode.BulletPos:
                    room.gameManager.SyncBulletPos(pack.BulletPack);
                    break;
                case FuncCode.GameEnd:
                    Debug.Log("收到游戏结束消息");
                    room.gameManager.EndGame(pack.GameEndPack);
                    break;
                case FuncCode.PlayerDead:
                    Debug.Log("收到有玩家死亡消息");
                    room.gameManager.PlayerDead(pack.PlayerDeadSrcIp);
                    break;
                case FuncCode.FuncNone:
                default:
                    Log.LogError($"Could not found funcName:{pack.FuncCode}");
                    break;
            }
        }


        public override byte[] Encode(GamePack pack)
        {
            return base.Encode(m_ActionType, pack);
        }

        protected override GamePack Decode(byte[] msg_body)
        {
            var gamePack = GamePack.Parser.ParseFrom(msg_body);
            return gamePack;
        }
    
    }
}