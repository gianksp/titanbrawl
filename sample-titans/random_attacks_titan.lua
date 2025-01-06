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

-- Seed the random number generator
math.randomseed(os.time())

-- Initialize your titan with a name and a modality
init("Rayback", "energy")

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
   -- print("Attack iteration")
   -- If outside range move
   if action == "on_guard" and distanceToOpponent >= 1.5 then
      -- Get closer with step
      run("move_forward", 1)
   elseif action == "on_guard" and distanceToOpponent >= 1 then
      -- Get closer with a power attack
      local power_attack = power_attacks[math.random(1, #power_attacks)]
      run(power_attack, 1.2)
   elseif action == "on_guard" and energy >= 50 then
      -- Within 1.2m and high energy, do combo
      local combo_attack = combo_attacks[math.random(1, #combo_attacks)]
      run(combo_attack, 1.2)
   else
      -- Within range and low energy, basic attacks
      local next_attack = basic_attacks[math.random(1, #basic_attacks)]
      run(next_attack, 1.2)
   end
end
