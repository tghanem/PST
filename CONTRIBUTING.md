## What to contribute?

At the present time, and since the project is still in the read-only phase, the best type of contributions we can get are cases that we do not handle. Every case should be documented by a test to make sure that we cover it.

## How to contribute?

* Send me an email (at timothy.ghanim@gmail.com) with a sample MSG file that contains the test case and answers to the following questions:

	* What property the library fails to read (i.e. property ID and type)?
	* Where to find the property, which object has the property?
	* What is the execpted value of the property?

* Fork the repository, add your tests cases and send me a pull request.

## Notes when generating test messages

When you generate a message for a test case, the message subject should be the description of the case that we are testing. For example, if i'm writing a test to read 2 KB body, then the message subject will be TestRead2KSubject so that inside the test we can locate the message and attempt to read the subject property from it.

## Testing philosophy

* The tests are written in an end-to-end fashion so that we have the maximum flexibility to refactor the code inside. Every test should follow the following steps:

	* Open the PST file under test.
	* Navigate to the object that contains the property we want to read.
	* Read the property.
	* Assert that the property value is correct.
	
### Please don't forget to send me any questions/suggestions you have to timothy.ghanim@gmail.com.
