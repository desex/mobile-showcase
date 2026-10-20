UI Particle System

Version: 4.2.0
Author: Tier Studios

--------------------------------------------------------------------------------
OVERVIEW
--------------------------------------------------------------------------------
UI Particle System is a lightweight, script-based particle solution built specifically for the Unity UI (Canvas). 

Unlike standard 3D particles, this tool renders particles directly as UI Geometry (MaskableGraphic). This means you can easily create and control advanced effects inside your UI without the need for Render Textures, World Cameras, or complex layer setups.

Designed with a custom Inspector interface similar to Unity's built-in Shuriken System, so you will feel right at home.

--------------------------------------------------------------------------------
WHATS NEW IN V4.2.0
--------------------------------------------------------------------------------
[NEW] Inherit Velocity:
• Particles can pick up the emitter's motion, like Unity Shuriken
• Initial: add emitter velocity when a particle is born (swipe, coin fly, moving buttons)
• Current: World-space particles keep traveling with a moving UI element each frame
• Multiplier control, mapped by the Clone tool from Unity Particle Systems

[NEW] Align To Direction & Rotation By Speed:
• Billboard particles can face their velocity (sparks, arrows, slashes)
• Start Rotation is kept as an extra spin offset
• Rotation By Speed adds angular velocity from a speed range
• Stretched Billboard still faces velocity as before

[NEW] UI Particle Trigger:
• Add Component → UI → UI Particle Trigger
• Play / restart on enable, click, or pointer enter
• Optional burst on click and stop on disable / pointer exit
• Assign a target UI Particle System, or use the one on the same GameObject
• Pointer events need a Graphic with Raycast Target (Button, Image, or the particle system)

[NEW] CanvasGroup / parent fade:
• Particle and trail meshes now follow CanvasGroup and parent group alpha
• Overlay UI fades no longer leave particles fully opaque

[NEW] Shape: Sprite, Image & Text:
• Spawn from a sprite silhouette (alpha threshold, fill or edge)
• Spawn from a UI Image drawing rect (preserveAspect aware)
• Spawn from UI Text or TextMeshProUGUI letter silhouettes (logo / title effects)
• Emit From Edge traces outlines; Threshold ~0.5 works well with SDF fonts

[NEW] Collision (UI Bounds & Obstacles):
• Keep particles inside a RectTransform, or bounce off UI obstacle rects
• Bounce, dampen, friction, lifetime loss, kill-on-hit
• Sub-emitter and sound triggers for On Particle Collision

[IMPROVED] Clone Tool:
• Inherit Velocity, Align To Direction, Rotation By Speed, and Collision are converted instead of skipped
• Sprite / SpriteRenderer shapes map to Shape: Sprite

--------------------------------------------------------------------------------
WHATS NEW IN V4.1.2
--------------------------------------------------------------------------------
[NEW] Job System & Mesh Architecture (Major Overhaul):
• Complete rewrite of the core particle simulation logic
• Powered by Unity Job System for high-performance multi-threaded processing
• Single-Object Mesh rendering: Eliminated individual GameObject generation per particle
• Drastically reduced CPU overhead, Main Thread pressure, and Canvas rebuild costs
• Fully overhauled Simulation Space: World with rock-solid stability and zero bottlenecks

[NEW] Next-Gen Trail System:
• Completely redesigned to utilize native TrailRenderer drawing logic
• UI-optimized performance for heavy trail rendering
• 6 advanced trail color modes: Color, Random Color, Gradient, Random Gradient
• Dynamic inherit options: Inherit Particle Color and Inherit Texture Color

[NEW] Advanced Simulation Modules:
• Limit Velocity over Lifetime: Control and clamp particle speeds using curves or constants
• Force over Lifetime: Apply constant or curve-driven forces directly to UI particles
• Color by Speed & Size by Speed: Create dynamic visual feedback based on particle velocity
• Global Controls: Added native support for Simulation Speed and customized Delta Time

[NEW] Modernized Attractor Module:
• Fully upgraded particle attractor targeting behavior
• Added new "Direct Attractor" feature for precise, pinpoint particle targeting

[IMPROVED] User Interface & Experience:
• Redesigned Shuriken-style simplified Inspector UI
• Layout carefully matched to Unity's native Particle System for a zero learning curve

[IMPROVED] Particle System Clone Tool:
• Significantly enhanced core stability for standard Unity Particle System conversions
• Generates much more accurate and robust UI Particle clones with comprehensive conversion reports

[IMPROVED] Backward Compatibility & Upgrade Workflow:
• Built-in automatic layout migration for existing prefabs
• Automatic CanvasRenderer attachment with friendly inspector warnings upon upgrading

--------------------------------------------------------------------------------
WHATS NEW IN V2.1.1
--------------------------------------------------------------------------------
[NEW] Sound Effects System:
• Add audio feedback to your particle effects
• 5 trigger types: OnParticleBirth, OnParticleDeath, OnParticleCollision, OnSystemStart, OnSystemStop
• Multiple audio clip support with Random, Sequential, or Single play modes
• Advanced controls: Volume range, Pitch variation, Spatial blend
• Performance settings: Max simultaneous sounds, Cooldown time, Play once option
• Audio Mixer integration support

[NEW] Sub-Emitters System:
• Create complex particle chains and reactions
• 4 trigger types: Birth, Death, Lifetime threshold, Collision
• 5 action types: PlaySystem, StopSystem, EmitBurst, ChangeColor, ChangeSize
• Inherit properties: Position, Color, Size from parent particles
• Probability control for random effects
• Mobile-optimized with max concurrent limits and cooldown settings

[NEW] Sprite Atlas System:
• Automatic sprite atlas management for better performance
• Dramatically reduces draw calls
• Auto-enables Sprite Atlas on project startup
• Manual pack button for final optimization
• Mobile-friendly atlas settings (2048x2048, compression quality 50)

[NEW] Blend Mode System:
• Alpha Blend: Standard transparent blending for natural effects
• Additive Blend: Glowing, luminous effects perfect for fire, magic, energy
• Universal shader with UI masking support
• Runtime switchable blend modes
• Stencil buffer support for advanced masking

[IMPROVED] Performance Optimizations:
• Better draw call batching with Sprite Atlas
• Optimized particle update loops
• Reduced Canvas rebuild cost
• Improved memory management
• Frame-rate independent updates

--------------------------------------------------------------------------------
WHATS NEW IN V1.1.1
--------------------------------------------------------------------------------
Major Performance Update

This version includes a major performance optimization pass focused on reducing CPU usage and improving frame stability.

What's improved:
• Optimized particle update loop
• Reduced Canvas rebuild cost
• Improved batching behavior
• Better memory usage
• Smoother performance on low-end devices

This update makes the system more suitable for:
• Mobile games
• UI-heavy menus
• Performance-sensitive projects

--------------------------------------------------------------------------------
WHATS NEW IN V1.1.0
--------------------------------------------------------------------------------
[NEW] Velocity Over Lifetime Module:
Added support for controlling particle speed and direction over time using Linear and Curve definitions. Perfect for smoke, gravity effects, and dynamic UI animations.

[NEW] Improved Prefabs:
Added new example prefabs showcasing the Velocity mechanics.

--------------------------------------------------------------------------------
FEATURES
--------------------------------------------------------------------------------
• Easy Setup:
  Right-click in Hierarchy → UI → UI Particle System.
  (Works just like creating a standard Image or Text object).

• Native Hierarchy Sorting:
  No "Sorting Layers" or "Order in Layer" headaches! Particles are rendered based on their sibling index in the Hierarchy, just like any native UI element.

• Advanced Masking Support:
  Fully compatible with Unity's standard "Mask" and optimized "RectMask2D" components.

• Canvas Compatibility:
  Works seamlessly with "Overlay", "Screen Space - Camera", and "World Space" canvas modes.

• Core Modules:
  - Emission Settings
  - Shape Options (Box, Circle, Line, Cone, Point, Edge, Sprite, Image, Text)
  - Velocity Over Lifetime (Linear & Curve)
  - Inherit Velocity (Initial / Current)
  - Collision (UI Bounds & Obstacles)
  - Color Over Lifetime
  - Size Over Lifetime
  - Rotation Over Lifetime
  - Align To Direction & Rotation By Speed
  - Texture Sheet Animation
  - Noise & Turbulence
  - Trails
  - Attractor
  - Sound Effects
  - Sub-Emitters
  - UI Particle Trigger (click / hover / OnEnable)
  - CanvasGroup alpha inheritance
  - Sprite Atlas Optimization
  - Blend Modes
  - Fade Out
  - Follow Mouse Support

--------------------------------------------------------------------------------
PACKAGE CONTENTS
--------------------------------------------------------------------------------
1. UI Particle System Script & Editor Tools
2. Demo Scene (Showcasing masking, velocity, and sorting)
3. 100+ Ready-to-Use Particle Prefabs 
   (Includes 25 base styles × 4+ color variations, plus new velocity examples)
4. Universal Shader with Alpha/Additive blend modes
5. Automatic Sprite Atlas management system

--------------------------------------------------------------------------------
HOW TO USE
--------------------------------------------------------------------------------
1. Create Particle System:
   - In Hierarchy: Right Click → Effects → UI Particle System
   - Or via Component: Add Component → "UI Particle System" to any UI object.

2. Customize:
   - Use the custom inspector to adjust Lifetime, Speed, Emission, Velocity, Sound, and Sub-Emitters.

3. Sorting:
   - To make particles appear in front/behind other UI elements, simply drag the object up or down in the Hierarchy view.

4. Sound Effects:
   - Enable Sound Effects in inspector
   - Add audio clips and choose trigger type
   - Adjust volume, pitch, and performance settings

5. Sub-Emitters:
   - Enable Sub-Emitters in inspector
   - Add entries with trigger and action types
   - Set target systems and inheritance properties
   - Use probability for randomized effects

6. Sprite Atlas:
   - Automatically managed, no setup required
   - Click "Pack" button if changes aren't visible
   - Significantly improves performance with multiple sprites

7. Blend Modes:
   - Alpha Blend: Use for smoke, clouds, natural effects
   - Additive Blend: Use for fire, energy, magic, glowing effects

8. UI Particle Trigger:
   - Add "UI Particle Trigger" to a Button (or any Graphic with Raycast Target)
   - Assign the UI Particle System to play, or leave empty to use the same object
   - Enable Play On Click, Play On Pointer Enter, Burst On Click, or Play On Enable
   - If Play On Enable is used, consider disabling Emit On Start on the target to avoid a double play

9. Inherit Velocity:
   - Initial: moving emitters spray in the swipe direction
   - Current: use World simulation space so particles ride with a moving panel or currency fly-to

10. CanvasGroup:
   - Fading a parent CanvasGroup now fades particles and trails with it

11. Emit from Text:
   - Shape → Text, then assign a UI Text or TextMeshProUGUI
   - Particles spawn from the visible letter silhouettes (use Emit From Edge for outlines)
   - Place the particle system near the title; Threshold ~0.5 is a good start for TMP SDF fonts

--------------------------------------------------------------------------------
NOTES & SUPPORT
--------------------------------------------------------------------------------
• Optimized for mobile and desktop UI performance.
• Prefabs are fully customizable (change textures, colors, parameters).
• Sprite Atlas automatically reduces draw calls for better performance.
• Sound system uses object pooling to prevent audio source spam.
• Sub-Emitters support complex particle chains and reactions.

www.tierstudios.com

If you encounter any issues or have feature requests, feel free to reach out:
Email: info@tierstudios.com | info.tierstudios@gmail.com | m.anil.altundag@gmail.com