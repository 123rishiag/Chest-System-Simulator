### Chest-System Simulator

A dynamic and interactive chest management system featuring scrollable chest slots, currency management, and engaging gameplay mechanics. The system is designed using **Service Locator**, **Dependency Injection**, **Model-View-Controller (MVC)**, **Observer Pattern**, **Object Pooling**, **State Machine**, and **Scriptable Objects**, ensuring scalability and maintainability.

---

## Gameplay Elements

### **1. Currency Types and Properties**
- **Currency Type**: Defines the type of currency, such as Coins or Gems.
- **Initial Value**: The starting value of the currency in the system.
- **Color**: Customizable color used for visual feedback and theming.
- **Sprite**: Associated image or icon representing the currency in the UI.

### **2. Chest Types and Properties**
| **Type**       | **Unlock Time (Minutes)** | **Unlock Currency Type** | **Minutes Unlocked by Single Currency** | **Rewards**                                    |
|-----------------|---------------------------|---------------------------|------------------------------------------|-----------------------------------------------|
| Common          | 15                        | Gems                      | 10                                       | Coins: 100-200, Gems: 10-20                   |
| Rare            | 30                        | Gems                      | 10                                       | Coins: 300-500, Gems: 20-40                   |
| Epic            | 60                        | Gems                      | 10                                       | Coins: 600-800, Gems: 45-60                   |
| Legendary       | 180                       | Gems                      | 10                                       | Coins: 1000-1200, Gems: 80-100                |

### **3. Chest Slots**
- **Slot Properties**:
  - **Minimum Chest Count**: Specifies how many chests will be created automatically at the start of the game (defined in `ChestConfig`).
  - **Maximum Chest Count**: Defines the maximum number of chests that can exist in the system at any given time (defined in `ChestConfig`).
- A scrollable list of slots for storing chests, with a minimum of 4 (Minimum Chest Count) slots.
- Dynamic pop-ups for full slots when attempting to add more chests than the available (Maximum Chest Count) slots.

### **4. Chest States**
- **Locked**: Chests are added but unlocking hasn't started.
- **Unlock Queue**: Chests queued for unlocking once the current timer finishes.
- **Unlocking**: Timer is running, progressing toward an unlock.
- **Unlocked**: Timer is complete, ready for reward collection.
- **Collected**: Rewards claimed, slot freed for new chests.

### **5. Chest Unlocking System**
- **Start Timer**: Unlock chests for free, with only one chest unlocking at a time.
- **Unlock with Currency**: Skip timers using gems (1 Currency Type = Time defined in `ChestConfig`) (1 Gem = 10 minutes).
- **Queue System**: Automatically unlocks chests present inside queue in sequence when slots are occupied.

### **6. UI and Feedback**
- **Currency Bar**: Prominent display of Currency such as Coins and Gems.
- **Chest Slots**: Dynamic chest visuals for Locked, Unlock Queue, Unlocking, Unlocked, and Empty states.
- **Pop-ups and Notifications**: Real-time guidance for insufficient currency, slot availability, and currency usage.

---

## Design Patterns and Programming Principles

### 1. **Service Locator**  
Centralizes service access, reducing dependencies. `GameService` is used to manage services like `EventService` and `SoundService` for a modular and scalable architecture.

### 2. **Dependency Injection**  
Improves flexibility by injecting services into controllers dynamically. Services like `EventService` are passed during initialization to ensure loose coupling.

### 3. **Model-View-Controller (MVC)**  
Separates logic, visuals, and control flow for UI, Currency and Chest:
- **Model** stores data.
- **View** displays visuals.
- **Controller** manages interactions.

### 4. **Observer Pattern**  
Enables decoupled, event-driven updates. `EventService` notifies listeners about currency transaction, UI changes, and more.

### 5. **Object Pooling**  
Optimizes memory usage by reusing chest objects, managed through `ChestPool`.

### 6. **State Machine**  
Manages chest states like Locked, Unlock Queue, Unlocking, Unlocked, and Collected for scalable behavior management.

### 7. **Scriptable Objects**  
Enhances modularity by storing independent data. `ChestConfig`, `CurrencyConfig` and `SoundConfig` define chest, currency and sound properties.

---

## Architectural Overview

Below is the block diagram illustrating the architecture:

![Architectural Overview](Documents/block_diagram.png)

---

## Services and Components

1. **GameService**: Centralized service manager for initializing and managing all other services.

2. **EventService**: Manages event-driven communication across services.
   - **EventController**: Inherits from EventController to manage event registrations and notifications.

3. **SoundService**: Handles audio feedback for unlocking, notifications, and other gameplay events.
   - **SoundType**: Enum defining sound types.
   - **SoundConfig**: Centralizes audio configurations.
     
4. **UIService**: Manages user interactions and updates the visual interface dynamically.
   - **UIController**: Manages user interactions and dynamic UI updates like button clicks.
   - **UIView**: Renders UI elements dynamically.
     
5. **CurrencyService**: Handles the management and runtime operations of in-game currencies.
   - **CurrencyController**: Manages currency logic such as adding and deducting values.
   - **CurrencyModel**: Stores runtime currency data.
   - **CurrencyView**: Renders currency visuals dynamically.
   - **CurrencyConfig**: Defines initial values and properties for currencies.
   - **CurrencyType**: Enum defining currency types (Coins, Gems).
     
6. **ChestService**: Manages chest logic, states, and unlocking mechanisms.
   - **ChestPool**: Optimizes memory usage by reusing chest objects.
   - **ChestController**: Handles chest logic and manages state transitions.
   - **ChestModel**: Stores runtime data for individual chests.
   - **ChestView**: Manages the UI rendering of chests.
   - **ChestConfig**: Defines chest properties, such as rewards and timers.
   - **ChestType**: Enum defining chest types (Common, Rare, Epic, Legendary) and states (Locked, Unlock Queue, Unlocking, Unlocked, Collected).
   - **IChestState**: Interface that defines common behaviors for all chest states. These states are initialized by a child of `ChestGenericStateMachine`, such as `ChestStateMachine`.
     - **ChestLockedState**: Represents the state where a chest is locked, and the unlock process has not started.
     - **ChestUnlockQueueState**: Represents the state where the chest is in a queue, waiting for the unlock process to begin.
     - **ChestUnlockingState**: Represents the state where the chest's unlock timer is active.
     - **ChestUnlockedState**: Represents the state where the chest is ready for rewards to be collected.
     - **ChestCollectedState**: Represents the state where rewards have been claimed, and the chest is removed.
   - **ChestGenericStateMachine**: A generic state machine designed to manage state transitions for `ChestController`. This design allows for future implementation of chest-specific state machines (e.g., `ChestRareStateMachine`, `ChestEpicStateMachine`) if subcontrollers of `ChestController` (e.g., `ChestRareController`) are introduced based on chest type.
     - **ChestStateMachine**: A specific implementation of `ChestGenericStateMachine` that manages the behavior of a chest. This is created by the `ChestController`.

7. **Utilities**: Provides reusable generic utilities for enhanced code efficiency.
   - **GenericObjectPool**: Manages object pooling for optimal memory usage and performance.

---

## Events

1. **OnPlaySoundEffectEvent**: Plays a specific sound effect (Handled by `SoundService`).

2. **OnGetUIControllerEvent**: Retrieves the active UI Controller (Handled by `UIService`).

3. **OnGetCurrencyControllerEvent**: Fetches the Currency Controller for a specific `CurrencyType` (Handled by `CurrencyService`).

---

## Script and Asset Hierarchy

1. **Scripts**:
   - Organized by feature: `Utilites`, `Main`, `Event`, `Sound`, `UI`, `Currency`, `Chest` etc.

2. **Assets**:
   - **Sounds**: From Outscal.
   - **Prefabs**: Self-created for UI components.
   - **Art**:
     - `Art/Images`: Coin, gem, and chest images were downloaded from www.pngwing.com.

---

## Development Workflow

### Branches

1. **Feature-0-Project-Setup**: Set up the Unity project structure with folders for Assets, Scripts, Prefabs, and Scenes.  
2. **Feature-1-Chest-System-UI**: Developed prefabs and core UI elements, including a scrollable list for chest slots, pop-ups for notifications, main menu, and a chest processing panel popup.  
3. **Feature-2-Currency-System**: Created currency types using Scriptable Objects to define data such as type, initial value, color, and sprite, along with logic for currency management.  
4. **Feature-3-Chest-System**: Implemented chest types, states, unlocking logic, queue management, and undo functionality. Used Scriptable Objects to define chest configurations such as rewards, timers, and unlock costs.  
5. **Feature-4-Service-Locator-DI**: Centralized service access using Dependency Injection with Service Locator to ensure modular and decoupled architecture.  
6. **Feature-5-MVC-Architecture**: Refactored systems into MVC structure, ensuring separation of concerns for UI, Currency, and Chest systems.  
7. **Feature-6-Observer-Pattern**: Enabled decoupled communication between services using `EventService`.
8. **Feature-7-Object-Pooling**: Used object pooling to reuse chest objects, reducing memory overhead and improving performance.  
9. **Feature-8-State-Machine**: Integrated a State Machine to handle chest states like Locked, Unlock Queue, Unlocking, Unlocked, and Collected, enabling scalable state management.  
10. **Feature-9-Sound-Service**: Added immersive sound effects for gameplay feedback, such as unlocking, notifications, and rewards collection.  
11. **Feature-10-Finalization**: Replaced placeholders with polished assets, refined code for better maintainability, and improved visuals.  
12. **Feature-11-Documentation**: Prepared README with an overview, block diagrams, and branch descriptions.

---

## Setting Up the Project

1. Clone the Repository:  
   ```bash
   git clone https://github.com/123rishiag/Chest-System-Simulator.git
   ```
2. Open the project in Unity.

---

## Video Demo

[Watch the Gameplay Demo](https://www.loom.com/share/2abca6750b2a4c0dac3b9efa0f957870?sid=8e8173a7-51b1-49ae-87d5-9f8f1e2a8ae5)  
[Watch the Architecture Explanation](https://www.loom.com/share/26c0821d90c440258ed0df85375f6aee?sid=818dd8da-2f4d-41c1-a54b-772495b441d7)

---

## Play Link

[Play the Game](https://outscal.com/narishabhgarg/game/play-chest-system-simulator-game)

---