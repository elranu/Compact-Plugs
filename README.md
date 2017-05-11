# Compact Plugs & Compact Injection Beta

**Compact Plugs (CP)** is a small framework for make your applications extensible.  The idea of this framework is to support a [Plug-in Model](http://en.wikipedia.org/wiki/Plugin) for your application in .Net Framework and .Net Compact Framework. Compact Plugs depends of Compact Injection. Compact Plugs uses Compact Injection to inject the outputs of a Plugin into another one, at runtime.  A small project is included in this release to see how CP works called "TestCompactPlugs1". The main idea of Compact Plugs is that any component can be a plug-in of another plugin, with almost no coding.   Soon I will upload some documentation, fixes and improves of it.

**Compact Injection (CI)** is a really small DI Container, for .Net Compact Framework and .Net Framework.  Until now it only supports basic DI functions. It just supports injection for properties, not constructor nor methods injection.  
The property injection only supports:
* ValueTypes
* ReferenceTypes  - supports Dictionary and List Injections 
* Injection of other ObjectDefinitions

Features:
* CompactConstructor creates classes with private constructors, and can inject in private setters. 
* CompactCostructor can have several object definitions, of the same object, in different contexts. The default context is name “Default”.  Also for this feature you can create a Context selector, just implementing the interface IContextSelector.
* The configuration of the object definitions can be created by XML or by objects. 

![uml1](https://cloud.githubusercontent.com/assets/873616/25962738/4ce05234-3655-11e7-9d9f-ecd8e78eeeca.png)

* Has a Registry for singleton ObjectDefinitions
* CompactConstructor, has a Default constructor for your application (Singleton). Of course you can create as many CompactConstructor´s as you want.
You can invoke it like this: CompactConstructor.DefaultConstructor

When you download CompactContainer, you also download a project call TestInjection, which is an example, of how to use it.

Compact Plugs was call before Compact Container

---
My blog (http://www.ranu.com.ar)
