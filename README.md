ConstructiveKSP
===============

ConstructiveKSP is an add-on for the popular space program simulator Kerbal Space Program (https://kerbalspaceprogram.com)

Installation
============

This is a stand-alone add-on for KSP, you do not need any part files or otherwise. You simply need to compile the source into it's .dll target and drop it in the Plugins folder of KSP.

What it does
============

ConstructiveKSP aims to provide you with a live-running calculation of your rockets' deltaV as you build it. The numbers it gives are estimated, in that it does not take into account your path through the atmosphere, badly built rockets that will waste their fuel on vectoring, etc. For well built rockets however, this add-on should provide reasonably accurate deltaV values for each stage of your rocket.

Things to note
==============

This add-on completely ignores your custom staging, the game has an internal staging that it maintains regardless of what you do with your staging and that is what is used. Don't be surprised when you drag a few engines all into a single stage and the readout doesn't reflect that -- regardless of the configuration of your specific staging the calculations should remain (mostly) accurate.

Limitations
===========

Fuel lines are not currently supported. In most configurations this should not matter -- as long as there is an engine attached to each (chain of) fuel tank(s) the deltaV calculation should hold true. Only fuel tanks with no thrusters attached or attached via fuel lines to vastly different isp engines will break things.

Currently the calculations are all provided from at rest, which means that certain things like the Oberth effect are not modelled, either on Kerbin escape or during any gravity assists you would normally use for your inter-planetary journeys (for that matter, gravity assists aren't modelled either, for hopefully obvious reasons).

Contributing
============

If you're a player who finds this mod, feel free to submit bugs and feature requests directly via github. If you're a fellow modder, I fully encourage you to fork and submit a pull request with any changes you make.
