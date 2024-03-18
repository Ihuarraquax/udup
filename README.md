# Udup
> Unified documentation (???) project - just some buzzwords that make it look like this word stands for something.

## General Information
- Generate nice living documentation from code

## Road map

- [x] Display events and event handlers in a diagram
- [x] Find a way to collect information about places where events are dispatched
  - [ ] Get trace of all invocations that leads to event dispatch (e.g. Controller => Service => AggregateRoot => Method => Event)
- [ ] Find a way to generate udup data with building app (maybe use source generator?)
- [ ] Use syntax walker instead of linq