Material Interaction System

Copyright Kieren Wallace 2012

email: kierenwallace@senet.com.au

The material interaction system provides you with a kind of 'effects grid' with which you can spawn prefabs based on the physic materials of two colliding objects.

The system relies on the use of different physic materials for different types of effects. By default, colliders have no physic material assigned- you will have to add them manually.

To set up the effects grid, select 'Material Interaction Table' from the 'Window' menu. Then, register all the physic materials that you want handled by the system by clicking the dropdown next to 'Add or Remove Material Types' and dragging each material to the 'Add Material' field. You can use the 'Remove Material' field in the same way if you need to remove them later, or you can just click the 'X' next to each material in the grid (while the Add or Remove Materials dropdown is open).

When you've done that, drag a prefab to the 'Default Effect' field. This tells the system what to instantiate if there is nothing set up for any specific interaction, or if something tells it to use an unregistered physic material. WARNING: If you have nothing in this spot, you will leak empty gameObjects, because the system creates empty objects if it can't find a valid prefab.

Now, you can use the 'Striking / Receiving' matrix to set up what prefab will get instantiated for every given material interaction. When a script tells the system to instantiate an effect, it queries this table with the physic materials in question and spawns a copy of the prefab set up for that interaction.

If you want to make sure that the system is acting how you expect it to, use the 'Test Interactions' dropdown. This allows you to ask the table what prefab will be instantiated in a collision between two specified materials.

In the 'Settings' dialogue, you can change the location of the 'Table' asset that the material interaction system uses. To make sure that the table is always accessable from within a built executable, this path always appends '/Resources/Table.asset'- so what you are setting up is what folder that Resources directory will be inside.

From here you can also reset the entire table if something has gone badly wrong (which can happen if you delete physic materials after setting things up).

The simplest way to implement it on a rigidbody-controlled object is to add the line:

    MaterialInteractionSystem.SpawnEffect(collision, collider.sharedMaterial);

to any OnCollisionEnter callback.

If you are using some system other than physical collisions, as long as you have the physic materials of both objects as well as the point and normal that you want the effect spawned at, you can use

    MaterialInteractionSystem.SpawnEffect(strikingMaterial, RecievingMaterial, point, normal);

Use this variant for raycast bullet hits, for example.