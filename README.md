[![Watch the video](https://img.youtube.com/vi/z8mR668bAU4/maxresdefault.jpg)](https://youtu.be/z8mR668bAU4)

# Titan Brawl

**Titan Brawl** is an action-packed, code-driven robot boxing game where players become both coders and strategists. Program your *Titan* using **LUA scripts** to control its behavior, including attacks, defense, and movement. The arena isn't just about brute strength—it's about strategy, adaptability, and leveraging your Titan's energy and state while predicting your opponent's next move.

---

## 🎮 Gameplay Overview

In *Titan Brawl*, the goal isn't just to fight—it's to outthink your opponent. Code your Titan to respond dynamically to real-time events in the ring, using LUA scripts to design a fighting style that maximizes efficiency and effectiveness. Every decision matters: When do you attack? When do you defend? How do you adapt when your energy is low or your opponent has the upper hand?

If you've played games like *Robocode* or loved the mechanics of *Real Steel*, this game will give you a familiar yet refreshing experience.

---

## 🔥 Features

- **LUA-Based Programming**: Write scripts in LUA to program your Titan’s AI, determining how it moves, attacks, and defends in the ring.
- **Dynamic Energy Management**: Monitor your Titan’s energy levels in real-time and adjust strategies accordingly.
- **Opponent Awareness**: Code your Titan to analyze your opponent’s energy and behavior to predict and counter their moves.
- **Physics-Driven Combat**: Enjoy a blend of AI logic and Unity’s physics engine for realistic robot boxing.
- **Customizable Fight Strategies**: Experiment with different attack combos, defensive stances, and movement patterns to dominate the arena.
- **Sample Bots**: Access pre-written bots to test against or learn from.
- **Titan API**: Leverage a powerful scripting API to create advanced strategies.

---

## 🛠️Download and Play

- **Download the latest [release](https://github.com/gianksp/titanbrawl/releases)**:once downloaded, unzip it and double click on `Titan Brawl.exe` to run the game.
- **Read the API**: Familiariaze with the titan commands using lua in the API documentation below.
- **Test sample bots**: in the `/sample-titans` folder in this repo you can find some example codes.
- **Write your first titan**: using any text editor, write your lua titan code and load it in the arena.
- **Share your bot**: Feel free to post your titan code and mention me on [X](https://x.com/gianksp).
- **Challenge others**: Challenge other player titans to see who is the strongest fighter.

## 🤖 Sample Titans
We’ve included a few pre-written bots to help you get started or challenge your programming skills:

- **[RandomAttacksTitan](https://github.com/gianksp/titanbrawl/blob/main/sample-titans/random_attacks_titan.lua)**: Is a simple titan bot that closes distance to the opponent and randomly launches barrages of combos.
- **[CounterMovesTitan](https://github.com/gianksp/titanbrawl/blob/main/sample-titans/counter_moves_titan.lua)**: Is a passive bot that wait for opponents to get close so that it can counter attack.

## 📖API
In order to create a new titan script for Titan Brawl create a new empty `.txt` file, initialize your titan and write the behavior function.
### 1. Bootstrap the Titan 
`init(name, modality)`
Initialize your titan with a name and a modality. The name is a screen identifier that will help you know which titan is which (red or blue) and the modality is a type of titan that grants different passive attributes. If your titan is not initialized it will not be recognized.
```lua
-- Name, Modality
init("Rayback", "energy")
```
This `modality` feature is not yet implemented but will be as follow:
- **energy**: Has a 30% increased base energy recovery rate.
- **resillience**: Grants a 20% reduction on concussive blows.
- **speed**: Titan has a 10% faster speed base without any extra energy consumption.
- **power**: Grants 15% more damage per hit.

### 2. Implement the update function
`function update(action, energy, health, opponentAction, opponentEnergy, opponentHealth, distanceToOpponent)`
You must implement an update function that will receive the following attributes
- **action**: The current animation the titan is executing. These can be attacks triggered by the developer or reactions like `on_hit`.
- **energy**: Current energy from 0-100 of the titan.
- **health**: Current health points from 0-100 of the titan.
- **opponentAction**: The current animation the opponent is executing. These can be attacks triggered by the developer or reactions like `on_hit`.
- **opponentEnergy**: Current energy from 0-100 of the opponent.
- **opponentHealth**: Current health points from 0-100 of the opponent.
- **distanceToOpponent**: Distance between opponents in meters, calculated from the center of mass of each titan..

This function is invoked by the game client every `200 ms` and within the implementation of this function you can make your titan react to your opponent.

```lua
--- update runs every 200ms.
-- Receive status from your titan and your opponent's titan and also execute titan actions.
-- @param text action - on_guard as default guard or any of the actions above that can be invoked
-- @param number energy - current energy from 0 to 100
-- @param number health - current health from 0 to 100
-- @param text opponentAction - current opponent's action
-- @param number opponentEnergy - current opponent's energy from 0 to 100
-- @param number opponentHealth - current opponent's health from 0 to 100
-- @param number distanceToOpponent - distance between the titans in meters (center of mass)
-- @return
function update(action, energy, health, opponentAction, opponentEnergy, opponentHealth, distanceToOpponent)
    -- Your custom code goes here
end
```

### 3. Invoke a Titan action
In order to make your titan execute an action, you must call the `run(action, speed)`. Where the action can be found in the following [list of actions](https://raw.githubusercontent.com/gianksp/titanbrawl/refs/heads/main/unity-client/Assets/Resources/values.json). Where speed ranges from 1 to 2 (where 1 is base speed and 2 is double the base speed, at the expense of double the consumption of energy).

A titan can only run an action if it is in `on_guard` action state.

```lua
-- Actions available can be found at https://raw.githubusercontent.com/gianksp/titanbrawl/refs/heads/main/unity-client/Assets/Resources/values.json. This titan only moves forward if out of range.
function update(action, energy, health, opponentAction, opponentEnergy, opponentHealth, distanceToOpponent)
    -- Every 200ms command your titan to move forward at base speed if on  guard and further than 1.5 meters
    if action == "on_guard" and distanceToOpponent >= 1.5 then
      -- Get closer with step
      run("move_forward", 1)
    end
end
```

#### 3.1 Moving
Move your titan forward (closer to your opponent) or backwards (further away from opponent)
```lua
run("move_forward", 1)
run("move_backwards", 1)
```
#### 3.2 Defending
Titans have 3 separate attackable parts, the head, the chest and the abdomen. Mantaining a default guard protects against chest, low guard against abdomen and high guard against head attacks.
```lua
-- Set titan high guard to cover head attacks
set_guard(1)

-- Set titan default guard to cover chest attacks
set_guard(0)

-- Set titan low guard to cover abdomen attacks
set_guard(-1)
```
##### 3.2.1 Blocking
While on a guarding stance, attacks to the area will be automatically blocked. Blocking an attack also increases energy regeneration for a short perdiod of time.
##### 3.2.2 Concussions
Certain powerful attacks can trigger the titan to stagger backwards and lose guard for a few seconds, equivalent to a stun.
#### 3.3 Attacking
There are different type of attacks or actions that can be invoked using the `run(action, speed)` function. Basic attacks are single motion attacks like jab or cross, power attacks involve both an attack and a movement and combo attacks include multiple sequential attacks within the same command.
##### 3.3.1 Basic Attacks
Most basic attacks, energy efficient, low damage.
```lua
-- For more attacks refer to https://raw.githubusercontent.com/gianksp/titanbrawl/refs/heads/main/unity-client/Assets/Resources/values.json
run("jab", 1)
run("cross", 1)
.
.
.
```
##### 3.3.2 Power Attacks
These attacks usually include a step forward that also closes the distance. They consume more energy than basic attacks but also deal more damage. Not all basic or attack combos can be performed as power attacks.
```lua
-- For more attacks refer to https://raw.githubusercontent.com/gianksp/titanbrawl/refs/heads/main/unity-client/Assets/Resources/values.json
run("move_forward_jab", 1)
run("move_forward_cross", 1)
.
.
.
```
##### 3.3.3 Combo Attacks
The most devastating yet expesive attacking move barrages. While they take time to perform and are prone to interruptions they are energy efficient.
```lua
-- For more attacks refer to https://raw.githubusercontent.com/gianksp/titanbrawl/refs/heads/main/unity-client/Assets/Resources/values.json
run("jab_cross_jab_cross_hook_hook_right", 1)
run("cross_hook_body_cross", 1)
.
.
.
```
### 4. Win conditions
The fight is one round of 90 seconds where the winner will be declared by either dropping the opponent's hp to 0 before its own gets to 0 or by having more hp at the end of the match when the time runs out.

## 📖Titan lua empty template

```lua
-- Available basic attacks
local basic_attacks = {
   "jab", 
   "jab_body", 
   "cross", 
   "cross_body", 
   "hook_left", 
   "hook_left_body", 
   "hook_right",
   "hook_right_body",
   "upper_left",
   "upper_left_body",
   "upper_right",
   "upper_right_body",
}

-- Single attacks that incorporate closing or opening distance
local power_attacks = {
   "move_forward_hook_cross_hook",
   "dodge_backwards_cross",
   "move_forward_jab",
   "move_forward_cross",
   "move_forward_hook_left",
   "move_forward_hook_right"
}

-- Barrage of attacks in a single motion
local combo_attacks = {
   "jab_cross",
   "jab_jab_cross",
   "jab_cross_hook",
   "jab_hook_right_cross",
   "cross_hook_body_cross",
   "jab_cross_hook_cross",
   "jab_cross_jab_cross_hook_hook_right"
}

-- Evasive maneuvers
local evades = {
   "dodge_left",
   "dodge_right",
   "dodge_backwards"
}

-- Confuse your opponents
local feints = {
   "feint_cross",
   "feint_jab",
   "feint_hook_right"
}

-- Widen or close distance to opponent
local moves = {
   "move_forward",
   "move_backwards"
}

-- Initialize your titan with a name and a modality
init("Custom Titan", "energy")

--- update runs every 200ms.
-- Receive status from your titan and your opponent's titan and also execute titan actions.
-- @param text action - on_guard as default guard or any of the actions above that can be invoked
-- @param number energy - current energy from 0 to 100
-- @param number health - current health from 0 to 100
-- @param text opponentAction - current opponent's action
-- @param number opponentEnergy - current opponent's energy from 0 to 100
-- @param number opponentHealth - current opponent's health from 0 to 100
-- @param number distanceToOpponent - distance between the titans in meters (center of mass)
-- @return
function update(action, energy, health, opponentAction, opponentEnergy, opponentHealth, distanceToOpponent)
    -- Your code implementation
end

```
