using System;
using System.Globalization;

namespace Microsoft.VisualStudio.TestTools.UnitTesting
{
    public static class Assert
    {
        // Summary:
        //     Verifies that two specified objects are equal. The assertion fails if the
        //     objects are not equal.
        //
        // Parameters:
        //   expected:
        //     The first object to compare. This is the object the unit test expects.
        //
        //   actual:
        //     The second object to compare. This is the object the unit test produced.
        //
        // Exceptions:
        //   Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException:
        //     expected is not equal to actual.
        public static void AreEqual(object expected, object actual);
        //
        // Summary:
        //     Verifies that two specified generic type data are equal. The assertion fails
        //     if they are not equal.
        //
        // Parameters:
        //   expected:
        //     The first generic type data to compare. This is the generic type data the
        //     unit test expects.
        //
        //   actual:
        //     The second generic type data to compare. This is the generic type data the
        //     unit test produced.
        //
        // Type parameters:
        //   T:
        //
        // Exceptions:
        //   Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException:
        //     expected is not equal to actual.
        public static void AreEqual<T>(T expected, T actual);
        //
        // Summary:
        //     Verifies that two specified doubles are equal, or within the specified accuracy
        //     of each other. The assertion fails if they are not within the specified accuracy
        //     of each other.
        //
        // Parameters:
        //   expected:
        //     The first double to compare. This is the double the unit test expects.
        //
        //   actual:
        //     The second double to compare. This is the double the unit test produced.
        //
        //   delta:
        //     The required accuracy. The assertion will fail only if expected is different
        //     from actual by more than delta.
        //
        // Exceptions:
        //   Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException:
        //     expected is different from actual by more than delta.
        public static void AreEqual(double expected, double actual, double delta);
        //
        // Summary:
        //     Verifies that two specified singles are equal, or within the specified accuracy
        //     of each other. The assertion fails if they are not within the specified accuracy
        //     of each other.
        //
        // Parameters:
        //   expected:
        //     The first single to compare. This is the single the unit test expects.
        //
        //   actual:
        //     The second single to compare. This is the single the unit test produced.
        //
        //   delta:
        //     The required accuracy. The assertion will fail only if expected is different
        //     from actual by more than delta.
        //
        // Exceptions:
        //   Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException:
        //     expected is not equal to actual.
        public static void AreEqual(float expected, float actual, float delta);
        //
        // Summary:
        //     Verifies that two specified objects are equal. The assertion fails if the
        //     objects are not equal. Displays a message if the assertion fails.
        //
        // Parameters:
        //   expected:
        //     The first object to compare. This is the object the unit test expects.
        //
        //   actual:
        //     The second object to compare. This is the object the unit test produced.
        //
        //   message:
        //     A message to display if the assertion fails. This message can be seen in
        //     the unit test results.
        //
        // Exceptions:
        //   Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException:
        //     expected is not equal to actual.
        public static void AreEqual(object expected, object actual, string message);
        //
        // Summary:
        //     Verifies that two specified strings are equal, ignoring case or not as specified.
        //     The assertion fails if they are not equal.
        //
        // Parameters:
        //   expected:
        //     The first string to compare. This is the string the unit test expects.
        //
        //   actual:
        //     The second string to compare. This is the string the unit test produced.
        //
        //   ignoreCase:
        //     A Boolean value that indicates a case-sensitive or insensitive comparison.
        //     true indicates a case-insensitive comparison.
        //
        // Exceptions:
        //   Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException:
        //     expected is not equal to actual.
        public static void AreEqual(string expected, string actual, bool ignoreCase);
        //
        // Summary:
        //     Verifies that two specified generic type data are equal. The assertion fails
        //     if they are not equal. Displays a message if the assertion fails.
        //
        // Parameters:
        //   expected:
        //     The first generic type data to compare. This is the generic type data the
        //     unit test expects.
        //
        //   actual:
        //     The second generic type data to compare. This is the generic type data the
        //     unit test produced.
        //
        //   message:
        //     A message to display if the assertion fails. This message can be seen in
        //     the unit test results.
        //
        // Type parameters:
        //   T:
        //
        // Exceptions:
        //   Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException:
        //     expected is not equal to actual.
        public static void AreEqual<T>(T expected, T actual, string message);
        //
        // Summary:
        //     Verifies that two specified doubles are equal, or within the specified accuracy
        //     of each other. The assertion fails if they are not within the specified accuracy
        //     of each other. Displays a message if the assertion fails.
        //
        // Parameters:
        //   expected:
        //     The first double to compare. This is the double the unit test expects.
        //
        //   actual:
        //     The second double to compare. This is the double the unit test produced.
        //
        //   delta:
        //     The required accuracy. The assertion will fail only if expected is different
        //     from actual by more than delta.
        //
        //   message:
        //     A message to display if the assertion fails. This message can be seen in
        //     the unit test results.
        //
        // Exceptions:
        //   Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException:
        //     expected is different from actual by more than delta.
        public static void AreEqual(double expected, double actual, double delta, string message);
        //
        // Summary:
        //     Verifies that two specified singles are equal, or within the specified accuracy
        //     of each other. The assertion fails if they are not within the specified accuracy
        //     of each other. Displays a message if the assertion fails.
        //
        // Parameters:
        //   expected:
        //     The first single to compare. This is the single the unit test expects.
        //
        //   actual:
        //     The second single to compare. This is the single the unit test produced.
        //
        //   delta:
        //     The required accuracy. The assertion will fail only if expected is different
        //     from actual by more than delta.
        //
        //   message:
        //     A message to display if the assertion fails. This message can be seen in
        //     the unit test results.
        //
        // Exceptions:
        //   Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException:
        //     expected is not equal to actual.
        public static void AreEqual(float expected, float actual, float delta, string message);
        //
        // Summary:
        //     Verifies that two specified objects are equal. The assertion fails if the
        //     objects are not equal. Displays a message if the assertion fails, and applies
        //     the specified formatting to it.
        //
        // Parameters:
        //   expected:
        //     The first object to compare. This is the object the unit test expects.
        //
        //   actual:
        //     The second object to compare. This is the object the unit test produced.
        //
        //   message:
        //     A message to display if the assertion fails. This message can be seen in
        //     the unit test results.
        //
        //   parameters:
        //     An array of parameters to use when formatting message.
        //
        // Exceptions:
        //   Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException:
        //     expected is not equal to actual.
        public static void AreEqual(object expected, object actual, string message, params object[] parameters);
        //
        // Summary:
        //     Verifies that two specified strings are equal, ignoring case or not as specified,
        //     and using the culture info specified. The assertion fails if they are not
        //     equal.
        //
        // Parameters:
        //   expected:
        //     The first string to compare. This is the string the unit test expects.
        //
        //   actual:
        //     The second string to compare. This is the string the unit test produced.
        //
        //   ignoreCase:
        //     A Boolean value that indicates a case-sensitive or insensitive comparison.
        //     true indicates a case-insensitive comparison.
        //
        //   culture:
        //     A System.Globalization.CultureInfo object that supplies culture-specific
        //     comparison information.
        //
        // Exceptions:
        //   Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException:
        //     expected is not equal to actual.
        public static void AreEqual(string expected, string actual, bool ignoreCase, CultureInfo culture);
        //
        // Summary:
        //     Verifies that two specified strings are equal, ignoring case or not as specified.
        //     The assertion fails if they are not equal. Displays a message if the assertion
        //     fails.
        //
        // Parameters:
        //   expected:
        //     The first string to compare. This is the string the unit test expects.
        //
        //   actual:
        //     The second string to compare. This is the string the unit test produced.
        //
        //   ignoreCase:
        //     A Boolean value that indicates a case-sensitive or insensitive comparison.
        //     true indicates a case-insensitive comparison.
        //
        //   message:
        //     A message to display if the assertion fails. This message can be seen in
        //     the unit test results.
        //
        // Exceptions:
        //   Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException:
        //     expected is not equal to actual.
        public static void AreEqual(string expected, string actual, bool ignoreCase, string message);
        //
        // Summary:
        //     Verifies that two specified generic type data are equal. The assertion fails
        //     if they are not equal. Displays a message if the assertion fails, and applies
        //     the specified formatting to it.
        //
        // Parameters:
        //   expected:
        //     The first generic type data to compare. This is the generic type data the
        //     unit test expects.
        //
        //   actual:
        //     The second generic type data to compare. This is the generic type data the
        //     unit test produced.
        //
        //   message:
        //     A message to display if the assertion fails. This message can be seen in
        //     the unit test results.
        //
        //   parameters:
        //     An array of parameters to use when formatting message.
        //
        // Type parameters:
        //   T:
        //
        // Exceptions:
        //   Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException:
        //     expected is not equal to actual.
        public static void AreEqual<T>(T expected, T actual, string message, params object[] parameters);
        //
        // Summary:
        //     Verifies that two specified doubles are equal, or within the specified accuracy
        //     of each other. The assertion fails if they are not within the specified accuracy
        //     of each other. Displays a message if the assertion fails, and applies the
        //     specified formatting to it.
        //
        // Parameters:
        //   expected:
        //     The first double to compare. This is the double the unit tests expects.
        //
        //   actual:
        //     The second double to compare. This is the double the unit test produced.
        //
        //   delta:
        //     The required accuracy. The assertion will fail only if expected is different
        //     from actual by more than delta.
        //
        //   message:
        //     A message to display if the assertion fails. This message can be seen in
        //     the unit test results.
        //
        //   parameters:
        //     An array of parameters to use when formatting message.
        //
        // Exceptions:
        //   Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException:
        //     expected is different from actual by more than delta.
        public static void AreEqual(double expected, double actual, double delta, string message, params object[] parameters);
        //
        // Summary:
        //     Verifies that two specified singles are equal, or within the specified accuracy
        //     of each other. The assertion fails if they are not within the specified accuracy
        //     of each other. Displays a message if the assertion fails, and applies the
        //     specified formatting to it.
        //
        // Parameters:
        //   expected:
        //     The first single to compare. This is the single the unit test expects.
        //
        //   actual:
        //     The second single to compare. This is the single the unit test produced.
        //
        //   delta:
        //     The required accuracy. The assertion will fail only if expected is different
        //     from actual by more than delta.
        //
        //   message:
        //     A message to display if the assertion fails. This message can be seen in
        //     the unit test results.
        //
        //   parameters:
        //     An array of parameters to use when formatting message.
        //
        // Exceptions:
        //   Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException:
        //     expected is different from actual by more than delta.
        public static void AreEqual(float expected, float actual, float delta, string message, params object[] parameters);
        //
        // Summary:
        //     Verifies that two specified strings are equal, ignoring case or not as specified,
        //     and using the culture info specified. The assertion fails if they are not
        //     equal. Displays a message if the assertion fails.
        //
        // Parameters:
        //   expected:
        //     The first string to compare. This is the string the unit test expects.
        //
        //   actual:
        //     The second string to compare. This is the string the unit test produced.
        //
        //   ignoreCase:
        //     A Boolean value that indicates a case-sensitive or insensitive comparison.
        //     true indicates a case-insensitive comparison.
        //
        //   culture:
        //     A System.Globalization.CultureInfo object that supplies culture-specific
        //     comparison information.
        //
        //   message:
        //     A message to display if the assertion fails. This message can be seen in
        //     the unit test results.
        //
        // Exceptions:
        //   Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException:
        //     expected is not equal to actual.
        public static void AreEqual(string expected, string actual, bool ignoreCase, CultureInfo culture, string message);
        //
        // Summary:
        //     Verifies that two specified strings are equal, ignoring case or not as specified.
        //     The assertion fails if they are not equal. Displays a message if the assertion
        //     fails, and applies the specified formatting to it.
        //
        // Parameters:
        //   expected:
        //     The first string to compare. This is the string the unit test expects.
        //
        //   actual:
        //     The second string to compare. This is the string the unit test produced.
        //
        //   ignoreCase:
        //     A Boolean value that indicates a case-sensitive or insensitive comparison.
        //     true indicates a case-insensitive comparison.
        //
        //   message:
        //     A message to display if the assertion fails. This message can be seen in
        //     the unit test results.
        //
        //   parameters:
        //     An array of parameters to use when formatting message.
        //
        // Exceptions:
        //   Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException:
        //     expected is not equal to actual.
        public static void AreEqual(string expected, string actual, bool ignoreCase, string message, params object[] parameters);
        //
        // Summary:
        //     Verifies that two specified strings are equal, ignoring case or not as specified,
        //     and using the culture info specified. The assertion fails if they are not
        //     equal. Displays a message if the assertion fails, and applies the specified
        //     formatting to it.
        //
        // Parameters:
        //   expected:
        //     The first string to compare. This is the string the unit test expects.
        //
        //   actual:
        //     The second string to compare. This is the string the unit test produced.
        //
        //   ignoreCase:
        //     A Boolean value that indicates a case-sensitive or insensitive comparison.
        //     true indicates a case-insensitive comparison.
        //
        //   culture:
        //     A System.Globalization.CultureInfo object that supplies culture-specific
        //     comparison information.
        //
        //   message:
        //     A message to display if the assertion fails. This message can be seen in
        //     the unit test results.
        //
        //   parameters:
        //     An array of parameters to use when formatting message.
        //
        // Exceptions:
        //   Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException:
        //     expected is not equal to actual.
        public static void AreEqual(string expected, string actual, bool ignoreCase, CultureInfo culture, string message, params object[] parameters);
        //
        // Summary:
        //     Verifies that two specified objects are not equal. The assertion fails if
        //     the objects are equal.
        //
        // Parameters:
        //   notExpected:
        //     The first object to compare. This is the object the unit test expects not
        //     to match actual.
        //
        //   actual:
        //     The second object to compare. This is the object the unit test produced.
        //
        // Exceptions:
        //   Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException:
        //     notExpected is equal to actual.
        public static void AreNotEqual(object notExpected, object actual);
        //
        // Summary:
        //     Verifies that two specified generic type data are not equal. The assertion
        //     fails if they are equal.
        //
        // Parameters:
        //   notExpected:
        //     The first generic type data to compare. This is the generic type data the
        //     unit test expects not to match actual.
        //
        //   actual:
        //     The second generic type data to compare. This is the generic type data the
        //     unit test produced.
        //
        // Type parameters:
        //   T:
        //
        // Exceptions:
        //   Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException:
        //     notExpected is equal to actual.
        public static void AreNotEqual<T>(T notExpected, T actual);
        //
        // Summary:
        //     Verifies that two specified doubles are not equal, and not within the specified
        //     accuracy of each other. The assertion fails if they are equal or within the
        //     specified accuracy of each other.
        //
        // Parameters:
        //   notExpected:
        //     The first double to compare. This is the double the unit test expects not
        //     to match actual.
        //
        //   actual:
        //     The second double to compare. This is the double the unit test produced.
        //
        //   delta:
        //     The required inaccuracy. The assertion fails only if notExpected is equal
        //     to actual or different from it by less than delta.
        //
        // Exceptions:
        //   Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException:
        //     notExpected is equal to actual or different from it by less than delta.
        public static void AreNotEqual(double notExpected, double actual, double delta);
        //
        // Summary:
        //     Verifies that two specified singles are not equal, and not within the specified
        //     accuracy of each other. The assertion fails if they are equal or within the
        //     specified accuracy of each other.
        //
        // Parameters:
        //   notExpected:
        //     The first single to compare. This is the single the unit test expects.
        //
        //   actual:
        //     The second single to compare. This is the single the unit test produced.
        //
        //   delta:
        //     The required inaccuracy. The assertion will fail only if notExpected is equal
        //     to actual or different from it by less than delta.
        //
        // Exceptions:
        //   Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException:
        //     notExpected is equal to actual or different from it by less than delta.
        public static void AreNotEqual(float notExpected, float actual, float delta);
        //
        // Summary:
        //     Verifies that two specified objects are not equal. The assertion fails if
        //     the objects are equal. Displays a message if the assertion fails.
        //
        // Parameters:
        //   notExpected:
        //     The first object to compare. This is the object the unit test expects not
        //     to match actual.
        //
        //   actual:
        //     The second object to compare. This is the object the unit test produced.
        //
        //   message:
        //     A message to display if the assertion fails. This message can be seen in
        //     the unit test results.
        //
        // Exceptions:
        //   Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException:
        //     notExpected is equal to actual.
        public static void AreNotEqual(object notExpected, object actual, string message);
        //
        // Summary:
        //     Verifies that two specified strings are not equal, ignoring case or not as
        //     specified. The assertion fails if they are equal.
        //
        // Parameters:
        //   notExpected:
        //     The first string to compare. This is the string the unit test expects not
        //     to match actual.
        //
        //   actual:
        //     The second string to compare. This is the string the unit test produced.
        //
        //   ignoreCase:
        //     A Boolean value that indicates a case-sensitive or insensitive comparison.
        //     true indicates a case-insensitive comparison.
        //
        // Exceptions:
        //   Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException:
        //     notExpected is equal to actual.
        public static void AreNotEqual(string notExpected, string actual, bool ignoreCase);
        //
        // Summary:
        //     Verifies that two specified generic type data are not equal. The assertion
        //     fails if they are equal. Displays a message if the assertion fails.
        //
        // Parameters:
        //   notExpected:
        //     The first generic type data to compare. This is the generic type data the
        //     unit test expects not to match actual.
        //
        //   actual:
        //     The second generic type data to compare. This is the generic type data the
        //     unit test produced.
        //
        //   message:
        //     A message to display if the assertion fails. This message can be seen in
        //     the unit test results.
        //
        // Type parameters:
        //   T:
        //
        // Exceptions:
        //   Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException:
        //     notExpected is equal to actual.
        public static void AreNotEqual<T>(T notExpected, T actual, string message);
        //
        // Summary:
        //     Verifies that two specified doubles are not equal, and not within the specified
        //     accuracy of each other. The assertion fails if they are equal or within the
        //     specified accuracy of each other. Displays a message if the assertion fails.
        //
        // Parameters:
        //   notExpected:
        //     The first double to compare. This is the double the unit test expects not
        //     to match actual.
        //
        //   actual:
        //     The second double to compare. This is the double the unit test produced.
        //
        //   delta:
        //     The required inaccuracy. The assertion fails only if notExpected is equal
        //     to actual or different from it by less than delta.
        //
        //   message:
        //     A message to display if the assertion fails. This message can be seen in
        //     the unit test results.
        //
        // Exceptions:
        //   Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException:
        //     notExpected is equal to actual or different from it by less than delta.
        public static void AreNotEqual(double notExpected, double actual, double delta, string message);
        //
        // Summary:
        //     Verifies that two specified singles are not equal, and not within the specified
        //     accuracy of each other. The assertion fails if they are equal or within the
        //     specified accuracy of each other. Displays a message if the assertion fails.
        //
        // Parameters:
        //   notExpected:
        //     The first single to compare. This is the single the unit test expects.
        //
        //   actual:
        //     The second single to compare. This is the single the unit test produced.
        //
        //   delta:
        //     The required inaccuracy. The assertion will fail only if notExpected is equal
        //     to actual or different from it by less than delta.
        //
        //   message:
        //     A message to display if the assertion fails. This message can be seen in
        //     the unit test results.
        //
        // Exceptions:
        //   Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException:
        //     notExpected is equal to actual or different from it by less than delta.
        public static void AreNotEqual(float notExpected, float actual, float delta, string message);
        //
        // Summary:
        //     Verifies that two specified objects are not equal. The assertion fails if
        //     the objects are equal. Displays a message if the assertion fails, and applies
        //     the specified formatting to it.
        //
        // Parameters:
        //   notExpected:
        //     The first object to compare. This is the object the unit test expects not
        //     to match actual.
        //
        //   actual:
        //     The second object to compare. This is the object the unit test produced.
        //
        //   message:
        //     A message to display if the assertion fails. This message can be seen in
        //     the unit test results.
        //
        //   parameters:
        //     An array of parameters to use when formatting message.
        //
        // Exceptions:
        //   Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException:
        //     notExpected is equal to actual.
        public static void AreNotEqual(object notExpected, object actual, string message, params object[] parameters);
        //
        // Summary:
        //     Verifies that two specified strings are not equal, ignoring case or not as
        //     specified, and using the culture info specified. The assertion fails if they
        //     are equal.
        //
        // Parameters:
        //   notExpected:
        //     The first string to compare. This is the string the unit test expects not
        //     to match actual.
        //
        //   actual:
        //     The second string to compare. This is the string the unit test produced.
        //
        //   ignoreCase:
        //     A Boolean value that indicates a case-sensitive or insensitive comparison.
        //     true indicates a case-insensitive comparison.
        //
        //   culture:
        //     A System.Globalization.CultureInfo object that supplies culture-specific
        //     comparison information.
        //
        // Exceptions:
        //   Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException:
        //     notExpected is equal to actual.
        public static void AreNotEqual(string notExpected, string actual, bool ignoreCase, CultureInfo culture);
        //
        // Summary:
        //     Verifies that two specified strings are not equal, ignoring case or not as
        //     specified. The assertion fails if they are equal. Displays a message if the
        //     assertion fails.
        //
        // Parameters:
        //   notExpected:
        //     The first string to compare. This is the string the unit test expects not
        //     to match actual.
        //
        //   actual:
        //     The second string to compare. This is the string the unit test produced.
        //
        //   ignoreCase:
        //     A Boolean value that indicates a case-sensitive or insensitive comparison.
        //     true indicates a case-insensitive comparison.
        //
        //   message:
        //     A message to display if the assertion fails. This message can be seen in
        //     the unit test results.
        //
        // Exceptions:
        //   Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException:
        //     notExpected is equal to actual.
        public static void AreNotEqual(string notExpected, string actual, bool ignoreCase, string message);
        //
        // Summary:
        //     Verifies that two specified generic type data are not equal. The assertion
        //     fails if they are equal. Displays a message if the assertion fails, and applies
        //     the specified formatting to it.
        //
        // Parameters:
        //   notExpected:
        //     The first generic type data to compare. This is the generic type data the
        //     unit test expects not to match actual.
        //
        //   actual:
        //     The second generic type data to compare. This is the generic type data the
        //     unit test produced.
        //
        //   message:
        //     A message to display if the assertion fails. This message can be seen in
        //     the unit test results.
        //
        //   parameters:
        //     An array of parameters to use when formatting message.
        //
        // Type parameters:
        //   T:
        //
        // Exceptions:
        //   Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException:
        //     notExpected is equal to actual.
        public static void AreNotEqual<T>(T notExpected, T actual, string message, params object[] parameters);
        //
        // Summary:
        //     Verifies that two specified doubles are not equal, and not within the specified
        //     accuracy of each other. The assertion fails if they are equal or within the
        //     specified accuracy of each other. Displays a message if the assertion fails,
        //     and applies the specified formatting to it.
        //
        // Parameters:
        //   notExpected:
        //     The first double to compare. This is the double the unit test expects not
        //     to match actual.
        //
        //   actual:
        //     The second double to compare. This is the double the unit test produced.
        //
        //   delta:
        //     The required inaccuracy. The assertion will fail only if notExpected is equal
        //     to actual or different from it by less than delta.
        //
        //   message:
        //     A message to display if the assertion fails. This message can be seen in
        //     the unit test results.
        //
        //   parameters:
        //     An array of parameters to use when formatting message.
        //
        // Exceptions:
        //   Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException:
        //     notExpected is equal to actual or different from it by less than delta.
        public static void AreNotEqual(double notExpected, double actual, double delta, string message, params object[] parameters);
        //
        // Summary:
        //     Verifies that two specified singles are not equal, and not within the specified
        //     accuracy of each other. The assertion fails if they are equal or within the
        //     specified accuracy of each other. Displays a message if the assertion fails,
        //     and applies the specified formatting to it.
        //
        // Parameters:
        //   notExpected:
        //     The first single to compare. This is the single the unit test expects not
        //     to match actual.
        //
        //   actual:
        //     The second single to compare. This is the single the unit test produced.
        //
        //   delta:
        //     The required inaccuracy. The assertion will fail only if notExpected is equal
        //     to actual or different from it by less than delta.
        //
        //   message:
        //     A message to display if the assertion fails. This message can be seen in
        //     the unit test results.
        //
        //   parameters:
        //     An array of parameters to use when formatting message.
        //
        // Exceptions:
        //   Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException:
        //     notExpected is equal to actual or different from it by less than delta.
        public static void AreNotEqual(float notExpected, float actual, float delta, string message, params object[] parameters);
        //
        // Summary:
        //     Verifies that two specified strings are not equal, ignoring case or not as
        //     specified, and using the culture info specified. The assertion fails if they
        //     are equal. Displays a message if the assertion fails.
        //
        // Parameters:
        //   notExpected:
        //     The first string to compare. This is the string the unit test expects not
        //     to match actual.
        //
        //   actual:
        //     The second string to compare. This is the string the unit test produced.
        //
        //   ignoreCase:
        //     A Boolean value that indicates a case-sensitive or insensitive comparison.
        //     true indicates a case-insensitive comparison.
        //
        //   culture:
        //     A System.Globalization.CultureInfo object that supplies culture-specific
        //     comparison information.
        //
        //   message:
        //     A message to display if the assertion fails. This message can be seen in
        //     the unit test results.
        //
        // Exceptions:
        //   Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException:
        //     notExpected is equal to actual.
        public static void AreNotEqual(string notExpected, string actual, bool ignoreCase, CultureInfo culture, string message);
        //
        // Summary:
        //     Verifies that two specified strings are not equal, ignoring case or not as
        //     specified. The assertion fails if they are equal. Displays a message if the
        //     assertion fails, and applies the specified formatting to it.
        //
        // Parameters:
        //   notExpected:
        //     The first string to compare. This is the string the unit test expects not
        //     to match actual.
        //
        //   actual:
        //     The second string to compare. This is the string the unit test produced.
        //
        //   ignoreCase:
        //     A Boolean value that indicates a case-sensitive or insensitive comparison.
        //     true indicates a case-insensitive comparison.
        //
        //   message:
        //     A message to display if the assertion fails. This message can be seen in
        //     the unit test results.
        //
        //   parameters:
        //     An array of parameters to use when formatting message.
        //
        // Exceptions:
        //   Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException:
        //     notExpected is equal to actual.
        public static void AreNotEqual(string notExpected, string actual, bool ignoreCase, string message, params object[] parameters);
        //
        // Summary:
        //     Verifies that two specified strings are not equal, ignoring case or not as
        //     specified, and using the culture info specified. The assertion fails if they
        //     are equal. Displays a message if the assertion fails, and applies the specified
        //     formatting to it.
        //
        // Parameters:
        //   notExpected:
        //     The first string to compare. This is the string the unit test expects not
        //     to match actual.
        //
        //   actual:
        //     The second string to compare. This is the string the unit test produced.
        //
        //   ignoreCase:
        //     A Boolean value that indicates a case-sensitive or insensitive comparison.
        //     true indicates a case-insensitive comparison.
        //
        //   culture:
        //     A System.Globalization.CultureInfo object that supplies culture-specific
        //     comparison information.
        //
        //   message:
        //     A message to display if the assertion fails. This message can be seen in
        //     the unit test results.
        //
        //   parameters:
        //     An array of parameters to use when formatting message.
        //
        // Exceptions:
        //   Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException:
        //     notExpected is equal to actual.
        public static void AreNotEqual(string notExpected, string actual, bool ignoreCase, CultureInfo culture, string message, params object[] parameters);
        //
        // Summary:
        //     Verifies that two specified object variables refer to different objects.
        //     The assertion fails if they refer to the same object.
        //
        // Parameters:
        //   notExpected:
        //     The first object to compare. This is the object the unit test expects not
        //     to match actual.
        //
        //   actual:
        //     The second object to compare. This is the object the unit test produced.
        //
        // Exceptions:
        //   Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException:
        //     notExpected refers to the same object as actual.
        public static void AreNotSame(object notExpected, object actual);
        //
        // Summary:
        //     Verifies that two specified object variables refer to different objects.
        //     The assertion fails if they refer to the same object. Displays a message
        //     if the assertion fails.
        //
        // Parameters:
        //   notExpected:
        //     The first object to compare. This is the object the unit test expects not
        //     to match actual.
        //
        //   actual:
        //     The second object to compare. This is the object the unit test produced.
        //
        //   message:
        //     A message to display if the assertion fails. This message can be seen in
        //     the unit test results.
        //
        // Exceptions:
        //   Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException:
        //     notExpected refers to the same object as actual.
        public static void AreNotSame(object notExpected, object actual, string message);
        //
        // Summary:
        //     Verifies that two specified object variables refer to different objects.
        //     The assertion fails if they refer to the same object. Displays a message
        //     if the assertion fails, and applies the specified formatting to it.
        //
        // Parameters:
        //   notExpected:
        //     The first object to compare. This is the object the unit test expects not
        //     to match actual.
        //
        //   actual:
        //     The second object to compare. This is the object the unit test produced.
        //
        //   message:
        //     A message to display if the assertion fails. This message can be seen in
        //     the unit test results.
        //
        //   parameters:
        //     An array of parameters to use when formatting message.
        //
        // Exceptions:
        //   Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException:
        //     notExpected refers to the same object as actual.
        public static void AreNotSame(object notExpected, object actual, string message, params object[] parameters);
        //
        // Summary:
        //     Verifies that two specified object variables refer to the same object. The
        //     assertion fails if they refer to different objects.
        //
        // Parameters:
        //   expected:
        //     The first object to compare. This is the object the unit test expects.
        //
        //   actual:
        //     The second object to compare. This is the object the unit test produced.
        //
        // Exceptions:
        //   Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException:
        //     expected does not refer to the same object as actual.
        public static void AreSame(object expected, object actual);
        //
        // Summary:
        //     Verifies that two specified object variables refer to the same object. The
        //     assertion fails if they refer to different objects. Displays a message if
        //     the assertion fails.
        //
        // Parameters:
        //   expected:
        //     The first object to compare. This is the object the unit test expects.
        //
        //   actual:
        //     The second object to compare. This is the object the unit test produced.
        //
        //   message:
        //     A message to display if the assertion fails. This message can be seen in
        //     the unit test results.
        //
        // Exceptions:
        //   Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException:
        //     expected does not refer to the same object as actual.
        public static void AreSame(object expected, object actual, string message);
        //
        // Summary:
        //     Verifies that two specified object variables refer to the same object. The
        //     assertion fails if they refer to different objects. Displays a message if
        //     the assertion fails, and applies the specified formatting to it.
        //
        // Parameters:
        //   expected:
        //     The first object to compare. This is the object the unit test expects.
        //
        //   actual:
        //     The second object to compare. This is the object the unit test produced.
        //
        //   message:
        //     A message to display if the assertion fails. This message can be seen in
        //     the unit test results.
        //
        //   parameters:
        //     An array of parameters to use when formatting message.
        //
        // Exceptions:
        //   Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException:
        //     expected does not refer to the same object as actual.
        public static void AreSame(object expected, object actual, string message, params object[] parameters);
        //
        // Summary:
        //     Do not use this method.
        //
        // Parameters:
        //   objA:
        //
        //   objB:
        public static bool Equals(object objA, object objB);
        //
        // Summary:
        //     Fails the assertion without checking any conditions.
        //
        // Exceptions:
        //   Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException:
        //     Always thrown.
        public static void Fail();
        //
        // Summary:
        //     Fails the assertion without checking any conditions. Displays a message.
        //
        // Parameters:
        //   message:
        //     A message to display. This message can be seen in the unit test results.
        //
        // Exceptions:
        //   Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException:
        //     Always thrown.
        public static void Fail(string message);
        //
        // Summary:
        //     Fails the assertion without checking any conditions. Displays a message,
        //     and applies the specified formatting to it.
        //
        // Parameters:
        //   message:
        //     A message to display. This message can be seen in the unit test results.
        //
        //   parameters:
        //     An array of parameters to use when formatting message.
        //
        // Exceptions:
        //   Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException:
        //     Always thrown.
        public static void Fail(string message, params object[] parameters);
        //
        // Summary:
        //     Indicates that the assertion cannot be verified.
        //
        // Exceptions:
        //   Microsoft.VisualStudio.TestTools.UnitTesting.AssertInconclusiveException:
        //     Always thrown.
        public static void Inconclusive();
        //
        // Summary:
        //     Indicates that the assertion can not be verified. Displays a message.
        //
        // Parameters:
        //   message:
        //     A message to display. This message can be seen in the unit test results.
        //
        // Exceptions:
        //   Microsoft.VisualStudio.TestTools.UnitTesting.AssertInconclusiveException:
        //     Always thrown.
        public static void Inconclusive(string message);
        //
        // Summary:
        //     Indicates that an assertion can not be verified. Displays a message, and
        //     applies the specified formatting to it.
        //
        // Parameters:
        //   message:
        //     A message to display. This message can be seen in the unit test results.
        //
        //   parameters:
        //     An array of parameters to use when formatting message.
        //
        // Exceptions:
        //   Microsoft.VisualStudio.TestTools.UnitTesting.AssertInconclusiveException:
        //     Always thrown.
        public static void Inconclusive(string message, params object[] parameters);
        //
        // Summary:
        //     Verifies that the specified condition is false. The assertion fails if the
        //     condition is true.
        //
        // Parameters:
        //   condition:
        //     The condition to verify is false.
        //
        // Exceptions:
        //   Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException:
        //     condition evaluates to true.
        public static void IsFalse(bool condition);
        //
        // Summary:
        //     Verifies that the specified condition is false. The assertion fails if the
        //     condition is true. Displays a message if the assertion fails.
        //
        // Parameters:
        //   condition:
        //     The condition to verify is false.
        //
        //   message:
        //     A message to display if the assertion fails. This message can be seen in
        //     the unit test results.
        //
        // Exceptions:
        //   Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException:
        //     condition evaluates to true.
        public static void IsFalse(bool condition, string message);
        //
        // Summary:
        //     Verifies that the specified condition is false. The assertion fails if the
        //     condition is true. Displays a message if the assertion fails, and applies
        //     the specified formatting to it.
        //
        // Parameters:
        //   condition:
        //     The condition to verify is false.
        //
        //   message:
        //     A message to display if the assertion fails. This message can be seen in
        //     the unit test results.
        //
        //   parameters:
        //     An array of parameters to use when formatting message.
        //
        // Exceptions:
        //   Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException:
        //     condition evaluates to true.
        public static void IsFalse(bool condition, string message, params object[] parameters);
        //
        // Summary:
        //     Verifies that the specified object is an instance of the specified type.
        //     The assertion fails if the type is not found in the inheritance hierarchy
        //     of the object.
        //
        // Parameters:
        //   value:
        //     The object to verify is of expectedType.
        //
        //   expectedType:
        //     The type expected to be found in the inheritance hierarchy of value.
        //
        // Exceptions:
        //   Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException:
        //     value is null or expectedType is not found in the inheritance hierarchy of
        //     value.
        public static void IsInstanceOfType(object value, Type expectedType);
        //
        // Summary:
        //     Verifies that the specified object is an instance of the specified type.
        //     The assertion fails if the type is not found in the inheritance hierarchy
        //     of the object. Displays a message if the assertion fails.
        //
        // Parameters:
        //   value:
        //     The object to verify is of expectedType.
        //
        //   expectedType:
        //     The type expected to be found in the inheritance hierarchy of value.
        //
        //   message:
        //     A message to display if the assertion fails. This message can be seen in
        //     the unit test results.
        //
        // Exceptions:
        //   Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException:
        //     value is null or expectedType is not found in the inheritance hierarchy of
        //     value.
        public static void IsInstanceOfType(object value, Type expectedType, string message);
        //
        // Summary:
        //     Verifies that the specified object is an instance of the specified type.
        //     The assertion fails if the type is not found in the inheritance hierarchy
        //     of the object. Displays a message if the assertion fails, and applies the
        //     specified formatting to it.
        //
        // Parameters:
        //   value:
        //     The object to verify is of expectedType.
        //
        //   expectedType:
        //     The type expected to be found in the inheritance hierarchy of value.
        //
        //   message:
        //     A message to display if the assertion fails. This message can be seen in
        //     the unit test results.
        //
        //   parameters:
        //     An array of parameters to use when formatting message.
        //
        // Exceptions:
        //   Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException:
        //     value is null or expectedType is not found in the inheritance hierarchy of
        //     value.
        public static void IsInstanceOfType(object value, Type expectedType, string message, params object[] parameters);
        //
        // Summary:
        //     Verifies that the specified object is not an instance of the specified type.
        //     The assertion fails if the type is found in the inheritance hierarchy of
        //     the object.
        //
        // Parameters:
        //   value:
        //     The object to verify is not of wrongType.
        //
        //   wrongType:
        //     The type that should not be found in the inheritance hierarchy of value.
        //
        // Exceptions:
        //   Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException:
        //     value is not null and wrongType is found in the inheritance hierarchy of
        //     value.
        public static void IsNotInstanceOfType(object value, Type wrongType);
        //
        // Summary:
        //     Verifies that the specified object is not an instance of the specified type.
        //     The assertion fails if the type is found in the inheritance hierarchy of
        //     the object. Displays a message if the assertion fails.
        //
        // Parameters:
        //   value:
        //     The object to verify is not of wrongType.
        //
        //   wrongType:
        //     The type that should not be found in the inheritance hierarchy of value.
        //
        //   message:
        //     A message to display if the assertion fails. This message can be seen in
        //     the unit test results.
        //
        // Exceptions:
        //   Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException:
        //     value is not null and wrongType is found in the inheritance hierarchy of
        //     value.
        public static void IsNotInstanceOfType(object value, Type wrongType, string message);
        //
        // Summary:
        //     Verifies that the specified object is not an instance of the specified type.
        //     The assertion fails if the type is found in the inheritance hierarchy of
        //     the object. Displays a message if the assertion fails, and applies the specified
        //     formatting to it.
        //
        // Parameters:
        //   value:
        //     The object to verify is not of wrongType.
        //
        //   wrongType:
        //     The type that should not be found in the inheritance hierarchy of value.
        //
        //   message:
        //     A message to display if the assertion fails. This message can be seen in
        //     the unit test results.
        //
        //   parameters:
        //     An array of parameters to use when formatting message.
        //
        // Exceptions:
        //   Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException:
        //     value is not null and wrongType is found in the inheritance hierarchy of
        //     value.
        public static void IsNotInstanceOfType(object value, Type wrongType, string message, params object[] parameters);
        //
        // Summary:
        //     Verifies that the specified object is not null. The assertion fails if it
        //     is null.
        //
        // Parameters:
        //   value:
        //     The object to verify is not null.
        //
        // Exceptions:
        //   Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException:
        //     value is null.
        public static void IsNotNull(object value);
        //
        // Summary:
        //     Verifies that the specified object is not null. The assertion fails if it
        //     is null. Displays a message if the assertion fails.
        //
        // Parameters:
        //   value:
        //     The object to verify is not null.
        //
        //   message:
        //     A message to display if the assertion fails. This message can be seen in
        //     the unit test results.
        //
        // Exceptions:
        //   Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException:
        //     value is null.
        public static void IsNotNull(object value, string message);
        //
        // Summary:
        //     Verifies that the specified object is not null. The assertion fails if it
        //     is null. Displays a message if the assertion fails, and applies the specified
        //     formatting to it.
        //
        // Parameters:
        //   value:
        //     The object to verify is not null.
        //
        //   message:
        //     A message to display if the assertion fails. This message can be seen in
        //     the unit test results.
        //
        //   parameters:
        //     An array of parameters to use when formatting message.
        //
        // Exceptions:
        //   Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException:
        //     value is null.
        public static void IsNotNull(object value, string message, params object[] parameters);
        //
        // Summary:
        //     Verifies that the specified object is null. The assertion fails if it is
        //     not null.
        //
        // Parameters:
        //   value:
        //     The object to verify is null.
        //
        // Exceptions:
        //   Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException:
        //     value is not null.
        public static void IsNull(object value);
        //
        // Summary:
        //     Verifies that the specified object is null. The assertion fails if it is
        //     not null. Displays a message if the assertion fails.
        //
        // Parameters:
        //   value:
        //     The object to verify is null.
        //
        //   message:
        //     A message to display if the assertion fails. This message can be seen in
        //     the unit test results.
        //
        // Exceptions:
        //   Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException:
        //     value is not null.
        public static void IsNull(object value, string message);
        //
        // Summary:
        //     Verifies that the specified object is null. The assertion fails if it is
        //     not null. Displays a message if the assertion fails, and applies the specified
        //     formatting to it.
        //
        // Parameters:
        //   value:
        //     The object to verify is null.
        //
        //   message:
        //     A message to display if the assertion fails. This message can be seen in
        //     the unit test results.
        //
        //   parameters:
        //     An array of parameters to use when formatting message.
        //
        // Exceptions:
        //   Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException:
        //     value is not null.
        public static void IsNull(object value, string message, params object[] parameters);
        //
        // Summary:
        //     Verifies that the specified condition is true. The assertion fails if the
        //     condition is false.
        //
        // Parameters:
        //   condition:
        //     The condition to verify is true.
        //
        // Exceptions:
        //   Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException:
        //     condition evaluates to false.
        public static void IsTrue(bool condition);
        //
        // Summary:
        //     Verifies that the specified condition is true. The assertion fails if the
        //     condition is false. Displays a message if the assertion fails.
        //
        // Parameters:
        //   condition:
        //     The condition to verify is true.
        //
        //   message:
        //     A message to display if the assertion fails. This message can be seen in
        //     the unit test results.
        //
        // Exceptions:
        //   Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException:
        //     condition evaluates to false.
        public static void IsTrue(bool condition, string message);
        //
        // Summary:
        //     Verifies that the specified condition is true. The assertion fails if the
        //     condition is false. Displays a message if the assertion fails, and applies
        //     the specified formatting to it.
        //
        // Parameters:
        //   condition:
        //     The condition to verify is true.
        //
        //   message:
        //     A message to display if the assertion fails. This message can be seen in
        //     the unit test results.
        //
        //   parameters:
        //     An array of parameters to use when formatting message.
        //
        // Exceptions:
        //   Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException:
        //     condition evaluates to false.
        public static void IsTrue(bool condition, string message, params object[] parameters);
        //
        // Summary:
        //     In a string, replaces null characters ('\0') with "\\0".
        //
        // Parameters:
        //   input:
        //     The string in which to search for and replace null characters.
        //
        // Returns:
        //     The converted string with null characters replaced by "\\0".
        public static string ReplaceNullChars(string input);
    }
}