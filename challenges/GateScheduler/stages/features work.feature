Feature: Scheduling
	Flight schedules are provided by the airlines.
	In order to improve the profitability and convenience of the airport,
	The system should schedule flights to gates efficiently.


	Scenario: Flights retain their scheduled gate when possible
   ......
	  Given the client adds gates:
	    | gate |
	    | A1   |
       | A2   |
	  And the system has flights:
	    | flight | arrives | departs| status |
	    | 170    | 7:00    | 8:00   | OnTime |
	  And the client sends flights:
	    | flight | arrives | status    |
	    | 170    |         | Cancelled |
     When the client requests arrivals
	  Then the response is:
	    | flight | gate | arrives | status |
     And the client requests departures
	  Then the response is:
	    | flight | gate | arrives | status |
       

   Scenario: Airport provides departure time
   
   Scenario: Carrier provides earliest allowed departure time

   Scenario: Expected flight turnaround time can vary based on craft type

   Scenario: Flights may not have an arrival or departure time

   Scenario: Flights are limited to terminals based on Carrier
       American, Delta -> Terminal A
       USAir, IcelandAir -> Terminal B
       Qantas, Delta -> Terminal C
       
 priority flights



Feature: Flight status

    Scenario: Flight status defaults to OnTime
      Given the client adds gates:
        | gate |
        | A1   |
      And the client adds flights:
        | flight | arrives | departs |
        | 170    | 7:00    | 8:00    |
      When the client requests arrivals
      Then the response is:
        | flight | gate | arrives | status |
        | 170    | A1   | 7:00    | OnTime |

    Scenario: Flights may be cancelled
      Given the client adds gates:
        | gate |
        | A1   |
      And the system has flights:
        | flight | arrives | departs | status |
        | 120    | 7:00    | 8:00    | OnTime |
      And the client adds flights:
        | flight | status    |
        | 120    | Cancelled |
      When the client requests flights
      Then the response is:
        | flight | arrives | departs | status    |
        | 120    | 7:00    | 8:00    | Cancelled |
      When the client requests arrivals
      Then the response is:
        | flight | gate | arrives | status |
      When the client requests departures
      Then the response is:
        | flight | gate | arrives | status |