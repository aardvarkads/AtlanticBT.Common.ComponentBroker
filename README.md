AtlanticBT.Common.ComponentBroker
=================================
Author: Eric Murphy

Lightweight .NET dependency injection library

Usage:
ComponentBrokerInstance.RetrieveComponent<T>() - Retrieve (or create if new) component instance. Default is to match interface to name of object to instantiate (IFoo to Foo).

