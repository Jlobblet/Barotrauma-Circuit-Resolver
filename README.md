# Barotrauma-Circuit-Resolver
In Barotrauma, components are updated in the order of their IDs. Each component is updated only once during each frame. This can lead to delays in the signal propagation or even break circuit operation entirely, as most components calculate and send their output only upon being updated. 

This circuit resolver removes these delays by redistributing the IDs over components such that components are only updated after the components that come before them have been updated. This is done using topological sorting.

## Prerequisites
Microsoft .NET Core 3.1

## Installation
Either build from source, or [download the compressed executable here](https://github.com/Jlobblet/Barotrauma-Circuit-Resolver/releases/tag/v1.0.0)

## Usage
Run `Barotrauma-Circuit-Resolver.exe`
