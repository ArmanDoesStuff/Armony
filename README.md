# Armony

A collection of useful scripts for use with the Unity engine.

## Utilities

### Audio
Clips can be played from anywhere via the `AudioMaster`, with options to include a sound type (effect, music, dialogue, etc.), all tied into a mixer.  
You can also simply call `.Play()` on a clip using the `AudioClipExtensions` class.

### Addressables
The `AddressableSpawner` takes an `AssetReference` and returns an awaitable `Task`. A much easier way of using the Addressables system.

### Serialization
Methods in the `DataOperations` class allow storing any kind of object via JSON serialization.  
The `SerializableDictionary` is a dictionary that can be accessed in the editor. It works with `AssetReference` and enums, unlike many similar classes.

### NetworkPool
The `NetworkPool` acts as a regular object pool but replicates any `NetworkPoolable` across the network. A `INetworkPoolUser` works using Unity's Netcode.  
You can easily sync the initial position and apply a constant transform to save on network overhead.

## Miscellaneous

### CameraShake
Convincing camera shake can be achieved by adding `CameraShake` to your camera and increasing the trauma.

### ConditionalCompilation
An easy way to ensure certain GameObjects only exist on specific platforms.

### Physics
`ConstantRotate`, `ConstantOscillate`, and `LookAtCamera` all apply simple transform operations.

### UI Extensions
1. `SnapInsideScrollOnSelect` ensures an object in a scroll rect is always visible when selected. Useful for controller support.
2. `MobileSafeAreaScaler` scales a rect to the safe area. Parent other canvas elements to it.
3. `SliderExtended` and `ButtonExtended` provide more events than the standard UI elements.

## Libraries

1. `LibGeneral` provides miscellaneous functions, including a particularly useful `.Random()` extension for any array.
2. `LibMathematics` contains physics methods like `AddExplosionForce` and straightforward utilities like `AngleBetweenTwoPoints`.
3. `LibUserInterface` includes standard functions and extensions like `.Fade()` to hide/show a canvas group.
4. `LibServer` makes it easy to specify which users should receive RPCs via `SendCaller` and `SendExceptCaller`.
5. `LibConversions` offers various conversions such as `HexToColor` and `.ToInt()` for booleans.
6. `LibAsync` provides an `AsyncOperation` extension that converts it into an awaitable `Task`.
