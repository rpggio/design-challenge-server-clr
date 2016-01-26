Feature: Time margins
  In order to maximize the use of gates for efficiency and convenience,
  the system should schedule flights at gates with adequate time margins.

  Background:
    Given the system is reset

  Scenario: Flights scheduled at a gate are offset by 30 minutes
    # Gates require turnover time between a flight departure and new arrival.
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
      | flight | gate | arrives | 
      | 140    | A1   | 4:00    |
      | 145    | A2   | 5:10    |
      | 150    | A1   | 5:30    |
