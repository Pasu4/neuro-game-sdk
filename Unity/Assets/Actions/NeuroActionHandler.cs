﻿#nullable enable

using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using NeuroSdk.Messages.Outgoing;
using NeuroSdk.Websocket;
using UnityEngine;

namespace NeuroSdk.Actions
{
    [PublicAPI]
    public sealed class NeuroActionHandler : MonoBehaviour
    {
        private static List<INeuroAction> _currentlyRegisteredActions = new();
        private static readonly List<INeuroAction> _dyingActions = new();

        public static INeuroAction? GetRegistered(string name) => _currentlyRegisteredActions.FirstOrDefault(a => a.Name == name);
        public static bool IsRecentlyUnregistered(string name) => _dyingActions.Any(a => a.Name == name);

        private void OnApplicationQuit()
        {
            WebsocketConnection.Instance!.SendImmediate(new ActionsUnregister(_currentlyRegisteredActions));
            _currentlyRegisteredActions = null!;
        }

        public static void RegisterActions(IReadOnlyCollection<INeuroAction> newActions)
        {
            _currentlyRegisteredActions.RemoveAll(oldAction => newActions.Any(newAction => oldAction.Name == newAction.Name));
            _dyingActions.RemoveAll(oldAction => newActions.Any(newAction => oldAction.Name == newAction.Name));
            _currentlyRegisteredActions.AddRange(newActions);
            WebsocketConnection.Instance!.Send(new ActionsRegister(newActions));
        }

        public static void RegisterActions(params INeuroAction[] newActions)
            => RegisterActions((IReadOnlyCollection<INeuroAction>) newActions);

        public static void UnregisterActions(IEnumerable<string> removeActionsList)
        {
            INeuroAction[] actionsToRemove = _currentlyRegisteredActions.Where(oldAction => removeActionsList.Any(removeAction => oldAction.Name == removeAction)).ToArray();

            _currentlyRegisteredActions.RemoveAll(actionsToRemove.Contains);
            _dyingActions.AddRange(actionsToRemove);
            removeActions().Forget();

            WebsocketConnection.Instance!.Send(new ActionsUnregister(removeActionsList));

            return;

            async UniTask removeActions()
            {
                await UniTask.Delay(10000);
                _dyingActions.RemoveAll(actionsToRemove.Contains);
            }
        }

        public static void UnregisterActions(IEnumerable<INeuroAction> removeActionsList)
            => UnregisterActions(removeActionsList.Select(a => a.Name));

        public static void UnregisterActions(params INeuroAction[] removeActionsList)
            => UnregisterActions((IReadOnlyCollection<INeuroAction>) removeActionsList);

        public static void UnregisterActions(params string[] removeActionNamesList)
            => UnregisterActions((IReadOnlyCollection<string>) removeActionNamesList);

        public static void ResendRegisteredActions()
        {
            WebsocketConnection.Instance!.Send(new ActionsRegister(_currentlyRegisteredActions));
        }
    }
}
