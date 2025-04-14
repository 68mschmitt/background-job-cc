Console.WriteLine("Hello World!");

/*
 * There are 3 implementations of the background workers
 * 1. Customer action execution (High through-put for processing emails, applying customer credits, and other customer actions)
 * 2. Employee uploader processing (large csv files will be uploaded to then be processed and entered into the db)
 * 3. Financial data sync (Maintain a state to track a cursor for syncing data from an external system)
 */

/*
 * Given the 3 interfaces to start
 * - database
 * - queue
 * - trigger
 *
 * I can infer what design is expected
 * - Database - Simple crud for interacting with the database
 * - Queue - Method to...
 *   - Queue a message
 *    - Implying that these are going to take a while and should be queued in the background
 *   - subscribe to messages
 *    - Functional classes (The class that will be doing the processing) will subscribe to the messages
 *   - Unsubscribe from messages
 * - Trigger
 */
