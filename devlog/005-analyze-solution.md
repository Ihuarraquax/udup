Now, goal is to create file with all required data to build living documentation at runtime

I'm digging into roslyn and how to efficiently extract data from code. I'm looking for all helpers to do that, to not get stuck with fighting edge cases with manual traversing.

I've found SymbolFinder class, which is full of helpful methods to find where given symbol is used