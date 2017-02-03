using System;
using System.Collections.Generic;

public class MessagingManager
{
	public static MessagingManager Instance = null;

    // public property for manager
    private List<Action> subscribers = new List<Action>();
    private List<Action<bool>> uiEventSubscribers = new List<Action<bool>>();
    private List<Action<InventoryItem>> inventorySubscribers = new List<Action<InventoryItem>>();

	void Awake()
	{
		if(Instance == null) Instance = this;
	}

    // Subscribe method for manager
    public void Subscribe(Action subscriber)
    {
        if (subscribers != null)
        {
            subscribers.Add(subscriber);
        }
    }

    // Unsubscribe method for manager
    public void UnSubscribe(Action subscriber)
    {
        if (subscribers != null)
        {
            subscribers.Remove(subscriber);
        }
    }

    // Clear subscribers method for manager
    public void ClearAllSubscribers()
    {
        if (subscribers != null)
        {
            subscribers.Clear();
        }
    }

    public void Broadcast()
    {
        if (subscribers != null)
        {
            foreach (var subscriber in subscribers.ToArray())
            {
                subscriber();
            }
        }
    }

    // Subscribe method for manager
    public void SubscribeUIEvent(Action<bool> subscriber)
    {
        if (uiEventSubscribers != null)
        {
            uiEventSubscribers.Add(subscriber);
        }
    }

    public void BroadcastUIEvent(bool uIVisible)
    {
        if (uiEventSubscribers != null)
        {
            foreach (var subscriber in uiEventSubscribers.ToArray())
            {
                subscriber(uIVisible);
            }
        }
    }

    // Unsubscribe method for UI manager
    public void UnSubscribeUIEvent(Action<bool> subscriber)
    {
        if (uiEventSubscribers != null)
        {
            uiEventSubscribers.Remove(subscriber);
        }
    }

    // Clear subscribers method for manager
    public void ClearAllUIEventSubscribers()
    {
        if (uiEventSubscribers != null)
        {
            uiEventSubscribers.Clear();
        }
    }

    // Subscribe method for Inventory manager
    public void SubscribeInventoryEvent(Action<InventoryItem> subscriber)
    {
        if (inventorySubscribers != null)
        {
            inventorySubscribers.Add(subscriber);
        }
    }

    // Broadcast method for Inventory manager
    public void BroadcastInventoryEvent(InventoryItem itemInUse)
    {
        foreach (var subscriber in inventorySubscribers)
        {
            subscriber(itemInUse);
        }
    }

    // Unsubscribe method for Inventory manager
    public void UnSubscribeInventoryEvent(Action<InventoryItem> subscriber)
    {
        if (inventorySubscribers != null)
        {
            inventorySubscribers.Remove(subscriber);
        }
    }

    // Clear subscribers method for Inventory manager
    public void ClearAllInventoryEventSubscribers()
    {
        if (inventorySubscribers != null)
        {
            inventorySubscribers.Clear();
        }
    }

}


