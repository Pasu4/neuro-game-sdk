# FAQ

Some of these questions were not answered officially, instead their answers were inferred by community members through observation.
These sources are marked as "unofficial".

## Is there a standard way / best practice for conveying dialogue?

> "Best practice" is 👍 do whatever you want. 👍
> 
> The context is being sent to Neuro and she will interpret it herself, as long as it's consistent throughout the game it doesn't matter how you signify speaker and other info.

[Source](https://github.com/VedalAI/neuro-game-sdk/issues/7#issuecomment-2533745279)

## How do I give Neuro persistent context?

Right now, Neuro is in charge of what she remembers. If it is very important, you can send reminders every so often.

[Source](https://github.com/VedalAI/neuro-game-sdk/issues/7#issuecomment-2536405147)

## How do I best send large amounts of data?

(Waiting on https://github.com/VedalAI/neuro-game-sdk/issues/43)

## Should I prefer multiple actions or a single action?

(Waiting on https://github.com/VedalAI/neuro-game-sdk/issues/43)

## What is the maximum length of a context message / action description / etc.?

An exact limit is not defined.
She can handle a decent amount, but sending too much ([such as the directory tree of a Unity project](https://github.com/VSC-NeuroPilot/neuropilot/issues/153)) will likely cause problems or crash her.

[Source (unofficial)](https://github.com/VedalAI/neuro-sdk/issues/43#issuecomment-3276628168)

<!-- Waiting on https://github.com/VedalAI/neuro-game-sdk/issues/43 for official confirmation -->

## How should the `state` of `actions/force` be formatted?

Any format that is serializable to text is accepted, however it is generally recommended to format `state` as Markdown.

[Source](https://github.com/VedalAI/neuro-sdk/blob/main/API/SPECIFICATION.md#force-actions)

## How much assistance should Neuro get?

However much would make for the best content.

[Source (unofficial)](https://github.com/VedalAI/neuro-sdk/issues/43#issuecomment-3276628168)

<!-- Waiting on https://github.com/VedalAI/neuro-game-sdk/issues/43 for official confirmation -->

## When should I use `actions/force`?

(Waiting on https://github.com/VedalAI/neuro-game-sdk/issues/43)

## When should I return a message with an `action/result`?

(Waiting on https://github.com/VedalAI/neuro-game-sdk/issues/43)

## What happens when an `actions/force` arrives while Neuro is busy?

This depends on the `priority` parameter of the `actions/force`.

[Source](https://github.com/VedalAI/neuro-sdk/blob/main/API/SPECIFICATION.md#force-actions)

## What happens if an `actions/force` gets filtered?

(Waiting on https://github.com/VedalAI/neuro-game-sdk/issues/49)

## Can I send context between receiving an `action` and sending the result?

(Waiting on https://github.com/VedalAI/neuro-sdk/issues/43#issuecomment-2854385371)

## How do I prevent Neuro from using a single-use action again without running into race conditions?

You can unregister actions before sending an action result. This ensures that by the time Neuro receives the result, she can no longer repeat an action you did not intend to allow anymore.

[Source](https://github.com/VedalAI/neuro-sdk/tree/main/API/README.md#unregister-disposable-actions-before-sending-result)

## How long can I wait to send back an `action/result`?

The `action/result` must be sent *as soon as possible*, meaning the only delay should be caused by network latency.

[Source 1](https://github.com/VedalAI/neuro-sdk/blob/main/API/SPECIFICATION.md#action-result)
| [Source 2 (unofficial)](https://github.com/VedalAI/neuro-sdk/issues/43#issuecomment-3276628168)

## Does a `silent: false` context packet guarantee that there will be a response?

No, the parameter simply changes how likely it is for Neuro to respond. It is not a guarantee.

[Source](https://github.com/VedalAI/neuro-sdk/blob/main/API/SPECIFICATION.md#context)

## What text that I send is added to her context?

Anything that is marked as something that Neuro will directly receive is added to her context. As of writing, this includes:

- Context messages ([Source](https://github.com/VedalAI/neuro-sdk/blob/main/API/SPECIFICATION.md#context))
- Action descriptions & schemas ([Source](https://github.com/VedalAI/neuro-sdk/blob/main/API/SPECIFICATION.md#action))
- Action force state & query ([Source](https://github.com/VedalAI/neuro-sdk/blob/main/API/SPECIFICATION.md#force-actions))
    - These are the only ones that can be *removed* from context, with the `ephemeral_context` parameter.
- Action result messages ([Source](https://github.com/VedalAI/neuro-sdk/blob/main/API/SPECIFICATION.md#action-result))

It is unknown how the other fields are handled.
