Feature: Time margins
  In order to maximize the use of gates for efficiency and convenience,
  the system should schedule flights at gates with adequate time margins.

  Background:
    Given the system is reset

  Scenario: Flights scheduled at a gate are offset by 30 minutes
    Given the client adds gates:
      | gate |
      | A1   |
      | A2   |
    And the client adds flights:
      | flight | arrives | departs |
      | 140    | 4:00    | 5:00    |
      | 145    | 5:10    | 6:10    |
      | 150    | 5:30    | 6:30    |
    When the client requests arrivals
    Then the response is:
      | flight | arrives | gate | 
      | 140    | 4:00    | A1   |
      | 145    | 5:10    | A2   |
      | 150    | 5:30    | A1   |

  Scenario: Arrival/departure events at neighboring gates are separated by at least 10 minutes
  # This will avoid traffic jams as passengers are loading and unloading at neighboring gates.
    Given the client adds gates:
      | gate |
      | A1   |
      | A2   |
      | A3   |
      | A4   |
      | A5   |
      | A6   |
    And the client adds flights:
      | flight | arrives | departs |
      | 170    | 8:00    | 9:00    |
      | 171    | 8:01    | 9:01    |
      | 172    | 9:02    | 10:02   |
      | 173    | 9:12    | 10:12   |
    When the client requests arrivals
    Then the response is:
      | flight | arrives | gate |
      | 170    | 8:00    | A1   |
      | 171    | 8:01    | A3   |
      | 172    | 9:02    | A5   |
      | 173    | 9:12    | A2   |
